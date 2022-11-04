using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly TwitPosterContext _context;
    private readonly ILogger<PostsController> _logger;
    
    public PostsController(TwitPosterContext context, ILogger<PostsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> Get()
    {
        return await _context.Posts.Include(p => p.Author).ToListAsync();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<Post> Create(CreatePostRequest request)
    {
        var post = new Post
        {
            AuthorId = this.GetUserId(),
            Body = request.Body,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created post by Author {AuthorId}", post.AuthorId);
        
        return post;
    }
}