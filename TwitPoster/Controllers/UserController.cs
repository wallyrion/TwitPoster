using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.Authentication;
using TwitPoster.ViewModels;

namespace TwitPoster.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly TwitPosterContext _context;
    private readonly ILogger<UserController> _logger;
    private readonly JwtTokenGenerator _tokenGenerator = new();

    public UserController(TwitPosterContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateUserRequest request)
    {
        try
        {
            var existedUser = await _context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);

            if (existedUser != null)
            {
                return BadRequest("Can not create user with existed email");
            }
            
            var user = new User
            {
                CreatedAt = DateTime.UtcNow,
                Email = request.Email,
                BirthDate = request.BirthDate,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var accessToken = _tokenGenerator.GenerateToken(user);

            return Ok(new {accessToken, user});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating user with Email {Email}", request.Email);
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "An error occurred while processing your request.");
        }
    }
}