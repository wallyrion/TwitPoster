using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Tests;

public class PostServiceTests
{
    private readonly TwitPosterContext _context;
    private readonly PostService _sut;
    private readonly Fixture _fixture = new();
    
    public PostServiceTests()
    {
        var options = new DbContextOptionsBuilder<TwitPosterContext>()
            .UseInMemoryDatabase($"DB{Guid.NewGuid()}")
            .Options;
        _context = new TwitPosterContext(options);
        _sut = new PostService(_context, null!);
    }

    [Fact]
    public async Task Like_Should_Increment()
    {
        var post = _fixture.Create<Post>();
        var previousLikes = post.LikesCount;
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var likes = await _sut.LikePost(post.Id);
        
        likes.Should().Be(previousLikes + 1 );
    }
    
    [Fact]
    public async Task Like_Should_Increment_When_Multiple_User_Liked()
    {
        var post = _fixture.Create<Post>();
        var previousLikes = post.LikesCount;
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var tasks = Enumerable.Range(0, 10).Select(_ => _sut.LikePost(post.Id));
        await Task.WhenAll(tasks);

        post.LikesCount.Should().Be(previousLikes + 10);
        
    }
}