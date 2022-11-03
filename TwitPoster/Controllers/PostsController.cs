using Microsoft.AspNetCore.Mvc;

namespace TwitPoster.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Post> Get()
    {
        return new List<Post>
        {
            new()
            {
                Author = "First", Body = "Hey", CreatedAt = DateTime.UtcNow, Id = 1
            },
            new()
            {
                Author = "Second", Body = "Hey", CreatedAt = DateTime.UtcNow.AddHours(-2), Id = 2
            }
        };
    }
}