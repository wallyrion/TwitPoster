using FluentAssertions;
using TwitPoster.DAL.Models;
using TwitPoster.Web.ViewModels.Post;

namespace TwitPoster.IntegrationTests.Post;

public class GetPostsTests : BaseIntegrationTest
{
    public GetPostsTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_Posts_Returns_Posts()
    {
        var expectedPosts = await Data.AddMany<DAL.Models.Post>();

        var postsResponse = await ApiClient.GetAsync("Posts");
        postsResponse.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<PostViewModel>>(x =>
            {
                x.Count.Should().Be(expectedPosts.Count);
                x.Should().BeEquivalentTo(expectedPosts, opt => opt.ExcludingMissingMembers());
            });
    }

    [Fact]
    public async Task Get_Posts_Returns_Posts_With_isLiked()
    {
        await AddAuthorization();
        var posts = await Data.AddMany<DAL.Models.Post>(10);
        var postIds = posts.Select(p => p.Id).ToArray();

        var likedPostIds = Random.Shared.GetItems(postIds.ToArray(), 5).Distinct().ToList();

        var postLikes = likedPostIds
            .Select(x => new PostLike
            {
                PostId = x,
                UserId = DefaultUserId
            });

        await Data.AddMany(postLikes.ToList());

        var postsResponse = await ApiClient.GetAsync("Posts");
        postsResponse.Should()
            .Be200Ok()
            .And
            .Satisfy<IReadOnlyList<PostViewModel>>(x =>
            {
                x.Count.Should().Be(posts.Count);
                var likedPosts = x.Where(p => p.IsLikedByCurrentUser).Select(p => p.Id).ToList();
                likedPosts.Should().BeEquivalentTo(likedPostIds);
            });
    }
}