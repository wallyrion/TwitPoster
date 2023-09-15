using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class LikePostTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Theory, AutoData]
    public async Task Like_post_concurrently_should_have_correct_LikesCount(CreatePostRequest createPostRequest)
    {
        await AddAuthorization();
        var postResponse = await ApiClient.PostAsJsonAsync("Posts", createPostRequest);
        postResponse.Should().Be200Ok();
        var post = await postResponse.Content.ReadFromJsonAsync<PostViewModel>();
        
        var concurrentUsersCount = 100;
        var clients = await CreateConcurrentClients(concurrentUsersCount);

        var results = await Task.WhenAll(clients.Select(x => 
            x.apiClient.PutAsync($"Posts/{post.Id}/like", null)));

        results.Should()
            .AllSatisfy(x => x.Should().Be200Ok());

        DbContext.PostLikes
            .Should().HaveSameCount(clients);
        
        DbContext.Posts.Should().ContainSingle()
            .Which.LikesCount.Should().Be(clients.Count);
    }

    [Theory, AutoData]
    public async Task UnLike_post_concurrently_should_have_correct_LikesCount(CreatePostRequest createPostRequest)
    {
        await AddAuthorization();
        var postResponse = await ApiClient.PostAsJsonAsync("Posts", createPostRequest);
        postResponse.Should().Be200Ok();
        var post = await postResponse.Content.ReadFromJsonAsync<PostViewModel>();
        
        var concurrentUsersCount = 100;
        var clients = await CreateConcurrentClients(concurrentUsersCount);

        var resultsLike = await Task.WhenAll(clients.Select(x => 
            x.apiClient.PutAsync($"Posts/{post.Id}/like", null)));

        resultsLike.Should()
            .AllSatisfy(x => x.Should().Be200Ok());
        
        var resultsUnlike = await Task.WhenAll(clients.Take(50).Select(x => 
            x.apiClient.PutAsync($"Posts/{post.Id}/unlike", null)));
        
        resultsUnlike.Should()
            .AllSatisfy(x => x.Should().Be200Ok());

        DbContext.PostLikes
            .Should().HaveSameCount(resultsUnlike);
        
        DbContext.Posts.Should().ContainSingle()
            .Which.LikesCount.Should().Be(resultsUnlike.Length);
    }
}
