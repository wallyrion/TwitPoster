using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.ViewModels;

namespace TwitPoster.Controllers;

[Route("[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly TwitPosterContext _context;

    public PostsController(TwitPosterContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> Get()
    {
        return await _context.Posts.Include(p => p.Author).ToListAsync();
    }
    
    [HttpPost]
    public async Task<Post> Create(CreatePostRequest request)
    {
        var post = new Post
        {
            AuthorId = 1,
            Body = request.Body,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return post;
    }
}