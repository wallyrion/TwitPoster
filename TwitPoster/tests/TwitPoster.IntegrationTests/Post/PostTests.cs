using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using TwitPoster.DAL;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests.Post;

public class PostTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _apiFactory;

    public PostTests(CustomerApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task GetPosts_Should_Return_Ok()
    {
        var client = _apiFactory.CreateClient();
        
        var posts = await client.GetAsync("Posts");

        posts.Should().BeSuccessful();
    }
}

public class CustomerApiFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .Build();

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
        });
        
       
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();


    }

    public new async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}
