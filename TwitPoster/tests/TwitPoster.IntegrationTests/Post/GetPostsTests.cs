using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

using TwitPoster.Web;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class GetPostsTests
{
    [Fact]
    public async Task Get_Posts_Returns_Posts()
    {
        var webFactory = new WebApplicationFactory<IApiTestMarker>();
        var client = webFactory.CreateClient();

        var postsResponse = await client.GetAsync("Posts");

        postsResponse.Should().BeSuccessful();
        var posts = await postsResponse.Content.ReadFromJsonAsync<IReadOnlyList<PostViewModel>>();
    }
}


/*.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
{
    var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<TwitPosterContext>));

    if (descriptor != null)
    {
        services.Remove(descriptor);
    }
                
    services.AddDbContext<TwitPosterContext>(options => options
        .UseSqlServer(_container.GetConnectionString()));
}));*/