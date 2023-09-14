using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class CreatePostsTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Create_Post_should_return_Unauthorized_for_anonymous()
    {
        var request = new CreatePostRequest("Post for integration tests");
        var createPostResponse = await ApiClient.PostAsJsonAsync("Posts", request);
        createPostResponse.Should().Be401Unauthorized();
    }

    [Theory, AutoData]
    public async Task Create_Post_should_create_post(CreatePostRequest createPostRequest)
    {
        await AddAuthorization();

        var postsResponse = await ApiClient.PostAsJsonAsync("Posts", createPostRequest);

        postsResponse.Should().Be200Ok().
            And.Satisfy<PostViewModel>(
            postViewmodel =>
                DbContext.Posts.Should().ContainSingle(p => p.Id == postViewmodel.Id)
                    .Which.Body.Should().Be(createPostRequest.Body)
        );
    }

    
    [Theory, AutoData]
    public async Task create_posts_concurrently_should_create_posts(CreatePostRequest createPostRequest)
    {
        var concurrentUsersCount = 10;
        var clients = await CreateConcurrentClients(concurrentUsersCount);
        
        var concurrentTasks = clients.Select(client => client.apiClient.PostAsJsonAsync("Posts", createPostRequest)).ToList();
        var results = await Task.WhenAll(concurrentTasks);

        results.Should()
            .AllSatisfy(x => x.Should().Be200Ok());

        var actualPosts = await DbContext.Posts.ToListAsync();
        actualPosts.Should().HaveSameCount(clients);
        actualPosts.Should().AllSatisfy(x => x.Body.Should().Be(createPostRequest.Body));
    }
}
