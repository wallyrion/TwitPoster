using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.IntegrationTests.Extensions;
using TwitPoster.IntegrationTests.ExternalApis;
using TwitPoster.IntegrationTests.ExternalApis.FakeResponses;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web;
using WireMock;
using WireMock.Types;
using WireMock.Util;

namespace TwitPoster.IntegrationTests.Location;

public class LocationTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly LocationApiServer _locationApiServer;
    private readonly IntegrationTestWebFactory _baseFactory;
    
    public LocationTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _baseFactory = factory;
        _locationApiServer = factory.LocationApiServer;
        _locationApiServer.Server.Reset();
        _locationApiServer.SetupCountries();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        await Factory.RedisContainer.ResetAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Retrieve_Countries_From_Cache(bool useDistributedCache)
    {
        var factoryWithCache = CreateFactory(useDistributedCache);
        var client = factoryWithCache.CreateClient();
        
        var countriesResponse = await client.GetAsync("/Location/countries");
        countriesResponse.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<Country>>(list => 
                list.Should().NotBeEmpty());
        _locationApiServer.Server.LogEntries.Should().ContainSingle()
            .Which.RequestMessage.Path.Should().Be("/api/v0.1/countries/flag/unicode");

        _locationApiServer.Server.ResetLogEntries();
        
        var countriesResponseFromCache = await client.GetAsync("/Location/countries");
        countriesResponseFromCache.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<Country>>(list => 
                list.Should().NotBeEmpty());

        _locationApiServer.Server.LogEntries.Should().BeEmpty("Countries should be retrieved from cache");

        var countriesOriginal = await countriesResponse.Content.ReadFromJsonAsync<IReadOnlyList<Country>>();
        var countriesFromCache = await countriesResponseFromCache.Content.ReadFromJsonAsync<IReadOnlyList<Country>>();

        countriesFromCache.Should().BeEquivalentTo(countriesOriginal);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task Should_Retrieve_Countries_Resilently_With_Retries(HttpStatusCode statusCode)
    {
        var requestsCount = 0;
        _locationApiServer.SetupCountries(_ =>
        {
            if (requestsCount++ < 2)
            {
                return new ResponseMessage
                {
                    StatusCode = (int)statusCode
                };
            }
            
            return new ResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                BodyData = new BodyData
                {
                    BodyAsString = CountriesResponseJson.All,
                    DetectedBodyType = BodyType.String
                },
                Headers = new Dictionary<string, WireMockList<string>>()
                {
                    {"Content-Type", "application/json"}
                }
            };
        });
        var factoryWithCache = CreateFactory(false);
        var client = factoryWithCache.CreateClient();
        
        var countriesResponse = await client.GetAsync("/Location/countries");
        countriesResponse.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<Country>>(list => 
                list.Should().NotBeEmpty());

        _locationApiServer.Server.LogEntries.Should().HaveCount(3, "Countries should be retrieved with 2 retries");
    }

    
    
    private WebApplicationFactory<IApiTestMarker> CreateFactory(bool useDistributedCache)
    {
        var factoryWithCache = _baseFactory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    var collection = new[]
                    {
                        KeyValuePair.Create("FeatureManagement:UseDistributedCache", useDistributedCache.ToString()),
                    };

                    configurationBuilder.AddInMemoryCollection(collection!);
                });
            });

        return factoryWithCache;
    }
}