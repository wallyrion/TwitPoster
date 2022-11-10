using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Mappers;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Middlewares;

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

    public async Task<List<PostDto>> GetPosts()
    {
        var postsF = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostLikes)
            .Select(p => new
            {
                p.Id,
                Count = p.PostLikes.Count,
                IsLiked = p.PostLikes.Any(pl => pl.UserId == _currentUser.Id),
            })
            .ToListAsync();
        
        var postsB = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostLikes)
            .Select(p => new
            {
                p.Id,
                Count = p.PostLikes.Count,
                IsLiked = p.PostLikes.Any(pl => pl.UserId == _currentUser.Id),
            })
            .ToListAsync();
        
        
        // var postsC = await _context.Posts
        //     .Include(p => p.Author)
        //     .Include(p => p.PostLikes)
        //     .Select(p => new PostDto(p.Id, p.Body, p.CreatedAt, p.Author.FirstName, p.Author.LastName, p.AuthorId, p.PostLikes.Count))
        //     .ToListAsync();

        var postsD = await _context.Posts
            .Include(p => p.Author)
            .Select(p => p.ToDto(p.PostLikes.Count, p.PostLikes.Any(l => l.UserId == _currentUser.Id)))
            .ToListAsync();

        var postsDy = await _context.Posts
            .Include(p => p.Author)
            .Include(e => e.PostLikes)
            .Select(p => p.ToDto())
            .ToListAsync();

        
        // var posts = await _context.Posts
        //     .Include(p => p.Author)
        //     .Include(p => p.PostLikes)
        //     .Select(p => p.ToDto())
        //     .ToListAsync();

        return postsD;
    }

    public async Task<PostDto> CreatePost(string body)
    {
        var isBanned = await _context.Users
            .Where(u => u.Id == _currentUser.Id)
            .Select(u => u.UserAccount.IsBanned)
            .FirstOrDefaultAsync();
        
        if (isBanned)
        {
            throw new TwitPosterValidationException ("You are banned");
        }
        
        var post = new Post
        {
            AuthorId = _currentUser.Id,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var createdPost = await _context.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == post.Id);
        return createdPost!.ToDto();
    }

    public async Task<List<PostComment>> GetComments(int postId)
    {
        return await _context.PostComments
            .Include(p => p.Author)
            .Where(c => c.PostId == postId)
            .ToListAsync();
    }

    public async Task<PostComment> CreateComment(int postId, string text)
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

        return savedComment!;
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
        
        return _context.PostLikes.Count(like => like.PostId == postId);
    }
}