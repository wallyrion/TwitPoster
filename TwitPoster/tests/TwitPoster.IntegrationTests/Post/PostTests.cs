using System.Net.Http.Json;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using TwitPoster.DAL;
using TwitPoster.Web;
using TwitPoster.Web.ViewModels;
using Xunit.Abstractions;

namespace TwitPoster.IntegrationTests.Post;

public class IntegrationFixture: IAsyncLifetime
{
    public readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    public readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();
    
    public IntegrationFixture()
    {
       
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }
}

public class IntegrationBase : IClassFixture<IntegrationFixture>
{
    public IntegrationBase(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
        var factory = new WebApplicationFactory<IApiTestMarker>().WithWebHostBuilder(builder =>
        {
            
            
            builder.ConfigureServices(services =>
            {

                services.AddLogging(x => x.SetMinimumLevel(LogLevel.Debug).AddXUnit(outputHelper));
                services.AddLogging(x => x.AddConsole());

                
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TwitPosterContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<TwitPosterContext>(options => options
                    .UseSqlServer(fixture._container.GetConnectionString()));

                var descriptorMassTransit = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(IBus));

                /*if (descriptor != null)
                {
                    services.Remove(descriptorMassTransit);
                }*/
                services.AddMassTransitTestHarness(x =>
                {
              
                });
                /*services.Configure<RabbitMqTransportOptions>(options =>
                {
                    options.Host = _rabbitMqContainer.Hostname;
                    options.Port = _rabbitMqContainer.GetMappedPublicPort("5672");
                });
                */

            });
            
            builder.ConfigureLogging(loggingBuilder =>
            {

                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                loggingBuilder.AddXUnit();
                loggingBuilder.AddConsole();
                
                loggingBuilder.Services.AddLogging(x => x.SetMinimumLevel(LogLevel.Debug).AddXUnit(outputHelper));
                loggingBuilder.Services.AddLogging(x => x.AddConsole());
                
            });
        });

        factory = factory.WithTestLogging(outputHelper);
        var client = factory.CreateClient();
    }
}

public class Tests : IntegrationBase
{
    public Tests(IntegrationFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }
    
    [Fact]
    public void Test()
    {
        
    }
    
    [Fact]
    public void Test2()
    {
        
    }
}


public class PostTests2 : IntegrationWebFactory
{

    public PostTests2(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public async Task GetPosts_Should_Return_Ok()
    {
        
    }
    
    [Fact]
    public async Task Register_Should_Return_Ok()
    {
        
    }
}


public class PostTests : BaseIntegrationTest
{
    private readonly  WebApplicationFactory<IApiTestMarker>  _webFactory;
    public PostTests(IntegrationTestWebFactory apiFactory, ITestOutputHelper outputHelper) : base(apiFactory)
    {
        _webFactory = apiFactory.WithTestLogging(outputHelper);
    }

    [Fact]
    public async Task GetPosts_Should_Return_Ok()
    {
        var client = _webFactory.CreateClient();
        
        var posts = await client.GetAsync("Posts");

        posts.Should().BeSuccessful();
    }
    
    [Fact]
    public async Task Register_Should_Return_Ok()
    {
        var client = Factory.CreateClient();

        var testHarness = Factory.Services.GetRequiredService<ITestHarness>();
        var registrationRequest = new RegistrationRequest("Milan", "Paris", DateTime.UtcNow.AddDays(-1), "kornienko1296@mgial.com", "Qwerty-1234456");
        var posts = await client.PostAsJsonAsync("Auth/registration", registrationRequest);

        var sent = await testHarness.Sent.Any();
        posts.Should().BeSuccessful();
    }
}

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>, IDisposable
{
    protected readonly IntegrationTestWebFactory Factory;
    
    public BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        Factory = factory;
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}


public class IntegrationWebFactory : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();
    
    public IntegrationWebFactory(ITestOutputHelper outputHelper)
    {
        

    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await _rabbitMqContainer.StartAsync();
        
        
        var factory = new WebApplicationFactory<IApiTestMarker>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TwitPosterContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<TwitPosterContext>(options => options
                    .UseSqlServer(_container.GetConnectionString()));

                var descriptorMassTransit = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(IBus));

                /*if (descriptor != null)
                {
                    services.Remove(descriptorMassTransit);
                }*/
                services.AddMassTransitTestHarness(x =>
                {
              
                });
                /*services.Configure<RabbitMqTransportOptions>(options =>
                {
                    options.Host = _rabbitMqContainer.Hostname;
                    options.Port = _rabbitMqContainer.GetMappedPublicPort("5672");
                });
                */

            });
        });
        
        var client = factory.CreateClient();

    }

    public async Task DisposeAsync()
    {
    }
}

public class IntegrationTestWebFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    public IntegrationTestWebFactory()
    {
        
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<TwitPosterContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<TwitPosterContext>(options => options
                .UseSqlServer(_container.GetConnectionString()));

            var descriptorMassTransit = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(IBus));

            /*if (descriptor != null)
            {
                services.Remove(descriptorMassTransit);
            }*/
            services.AddMassTransitTestHarness(x =>
            {
              
            });
            /*services.Configure<RabbitMqTransportOptions>(options =>
            {
                options.Host = _rabbitMqContainer.Hostname;
                options.Port = _rabbitMqContainer.GetMappedPublicPort("5672");
            });
            */

        });
        
       
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await _rabbitMqContainer.StartAsync();


    }

    public new async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }
}
