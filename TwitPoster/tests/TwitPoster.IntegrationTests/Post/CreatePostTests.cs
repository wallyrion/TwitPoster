using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using TwitPoster.DAL;
using TwitPoster.Web;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class CreatePostsTests : BaseIntegrationTest
{
    [Fact]
    public async Task Create_Post_should_return_Unauthorized_for_anonymous()
    {
        var request = new CreatePostRequest("Post for integration tests");
        var createPostResponse = await HttpClient.PostAsJsonAsync("Posts", request);
        createPostResponse.Should().Be401Unauthorized();
    }

    [Fact]
    public async Task Create_Post_should_create_post()
    {
        var msSqlContainer = new MsSqlBuilder().Build();
        await msSqlContainer.StartAsync();

        var webFactory = new WebApplicationFactory<IApiTestMarker>()
            .WithWebHostBuilder(b =>
            {
                b.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TwitPosterContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TwitPosterContext>(options => options
                        .UseSqlServer(msSqlContainer.GetConnectionString()));
                });
            });


        var request = new CreatePostRequest("Post for integration tests");
        var postsResponse = await HttpClient.PostAsJsonAsync("Posts", request);
        postsResponse.Should().BeSuccessful();
        var postResult = await postsResponse.Content.ReadFromJsonAsync<PostViewModel>();

        DbContext.Posts.Should()
            .ContainSingle(x => x.Id == postResult!.Id)
            .Which.Body.Should().Be(request.Body);
    }
}
