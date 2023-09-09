using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class GetPostsTests : BaseIntegrationTest
{
    [Fact]
    public async Task Get_Posts_Returns_Posts()
    {
        var fixture = new Fixture();
        var expectedPosts = fixture.Build<DAL.Models.Post>()
            .Without(p => p.Id)
            .Without(p => p.Author)
            .Without(p => p.Comments)
            .Without(p => p.PostLikes)
            .With(p => p.AuthorId, 1)
            .CreateMany()
            .ToList();

        DbContext.Posts.AddRange(expectedPosts);
        await DbContext.SaveChangesAsync();
        
        var postsResponse = await HttpClient.GetAsync("Posts");
        postsResponse.Should()
            .Satisfy<IReadOnlyList<PostViewModel>>(x =>
            {
                x.Count.Should().Be(expectedPosts.Count);
                x.Should().BeEquivalentTo(expectedPosts, opt => opt.ExcludingMissingMembers());
            });
    }

    [Theory, AutoData]
    public async Task Create_Post_should_create_post(CreatePostRequest createPostRequest)
    {
        await AddAuthorization();

        var postsResponse = await HttpClient.PostAsJsonAsync("Posts", createPostRequest);

        postsResponse.Should().Satisfy<PostViewModel>(
            postViewmodel =>
                DbContext.Posts.Should().ContainSingle(p => p.Id == postViewmodel.Id)
                    .Which.Body.Should().Be(createPostRequest.Body)
        );
    }
}

