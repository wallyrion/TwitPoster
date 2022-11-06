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
        var posts = await _context.Posts.Include(p => p.Author)
            .Select(p => p.ToDto())
            .ToListAsync();

        return posts;
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
}