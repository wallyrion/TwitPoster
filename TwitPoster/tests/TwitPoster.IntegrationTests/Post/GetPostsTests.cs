using AutoFixture;
using FluentAssertions;
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
}

