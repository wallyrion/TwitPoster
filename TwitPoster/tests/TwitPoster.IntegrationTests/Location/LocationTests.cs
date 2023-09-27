using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.IntegrationTests.ExternalApis;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests.Location;

public class LocationTests : BaseIntegrationTest
{
    private readonly LocationApiServer _locationApiServer;
    private readonly IntegrationTestWebFactory _baseFactory;
    
    public LocationTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _baseFactory = factory;
        _locationApiServer = factory.LocationApiServer;
        _locationApiServer.SetupCountries();
        _locationApiServer.Server.ResetLogEntries();
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
        
        _locationApiServer.Server.ResetLogEntries();
        
        var countriesResponseFromCache = await client.GetAsync("/Location/countries");
        countriesResponseFromCache.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<Country>>(list => 
                list.Should().NotBeEmpty());

        _locationApiServer.Server.LogEntries.Should().BeEmpty("Countries should be retrieved from cache");
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