using AutoFixture.Xunit2;
using FluentAssertions;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

[Collection(nameof(SharedTestCollection))]
public class CreatePostsTests : BaseIntegrationTest
{
    public CreatePostsTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }
    
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

}
