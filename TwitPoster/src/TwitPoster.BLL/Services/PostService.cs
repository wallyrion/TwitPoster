using Mapster;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Extensions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Services;

public class PostService : IPostService
{
    private readonly TwitPosterContext _context;
    private readonly ICurrentUser _currentUser;

    public PostService(TwitPosterContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PostDto>> GetPosts(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
    {
        TypeAdapterHelper.Override<Post, PostDto>(out var mapConfig)
            .Map(dest => dest.IsLikedByCurrentUser, src => src.PostLikes.Any(x => x.UserId == _currentUser.Id));

        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ProjectToType<PostDto>(mapConfig)
            .ToListAsync(cancellationToken);

        return posts;
    }

    public async Task<PostDto> CreatePost(string body)
    {
        var isBanned = await _context.Users
            .Where(u => u.Id == _currentUser.Id).Select(u => u.UserAccount.IsBanned).FirstOrDefaultAsync();

        if (isBanned)
        {
            throw new TwitPosterValidationException("You are banned");
        }

        var post = new Post
        {
            AuthorId = _currentUser.Id,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var createdPost = await _context.Posts
            .Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == post.Id);

        return createdPost!.Adapt<PostDto>();
    }

    public async Task<PostCommentDto> CreateComment(int postId, string text)
    {
        var post = await _context.Posts.FindAsync(postId);

        if (post == null)
        {
            throw new TwitPosterValidationException("Post not found");
        }

        var newComment = new PostComment
        {
            Text = text,
            AuthorId = _currentUser.Id,
            CreatedAt = DateTime.UtcNow,
            PostId = postId
        };

        await _context.PostComments.AddAsync(newComment);
        await _context.SaveChangesAsync();

        var savedComment = await _context.PostComments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == newComment.Id);

        return savedComment!.Adapt<PostCommentDto>();
    }

    public async Task<int> LikePost(int postId)
    {
        var post = await _context.Posts.FindAsync(postId);

        if (post == null)
        {
            throw new TwitPosterValidationException("Post not found");
        }

        var isLiked = await _context.PostLikes
            .AnyAsync(like => like.PostId == postId && like.UserId == _currentUser.Id);

        if (isLiked)
        {
            return _context.PostLikes.Count(like => like.PostId == postId);
        }

        var newLike = new PostLike
        {
            PostId = postId,
            UserId = _currentUser.Id
        };

        _context.PostLikes.Add(newLike);

        await _context.SaveChangesAsync();
        post.LikesCount = _context.PostLikes.Count(like => like.PostId == postId);
        await _context.SaveChangesAsync();

        return _context.PostLikes.Count(like => like.PostId == postId);
    }

    public async Task<int> UnlikePost(int postId)
    {
        var post = await _context.Posts.FindAsync(postId);

        if (post == null)
        {
            throw new TwitPosterValidationException("Post not found");
        }

        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == _currentUser.Id);

        if (existingLike == null)
        {
            return _context.PostLikes.Count(like => like.PostId == postId);
        }

        _context.PostLikes.Remove(existingLike);
        await _context.SaveChangesAsync();

        post.LikesCount = _context.PostLikes.Count(like => like.PostId == postId);
        await _context.SaveChangesAsync();

        return _context.PostLikes.Count(like => like.PostId == postId);
    }

    public async Task<PagedResult> GetComments(int postId, int pageSize, int pageNumber)
    {
        var posts = await _context.PostComments
            .Include(p => p.Author)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ProjectToType<PostCommentDto>()
            .ToListAsync();

        var totalCount = await _context.PostComments.CountAsync(c => c.PostId == postId);

        return new PagedResult(posts, totalCount);
    }

    public List<PostDto> GetPostsSync(int pageSize, int pageNumber)
    {
        TypeAdapterHelper.Override<Post, PostDto>(out var mapConfig)
            .Map(dest => dest.IsLikedByCurrentUser, src => src.PostLikes.Any(x => x.UserId == _currentUser.Id));

        var posts = _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ProjectToType<PostDto>(mapConfig)
            .ToList();

        return posts;
    }

    public async Task<int> GetPostsCount()
    {
        return await _context.Posts.CountAsync();
    }
}