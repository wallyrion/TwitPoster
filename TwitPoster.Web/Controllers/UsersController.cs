using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Authentication;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly TwitPosterContext _context;
    private readonly ILogger<UsersController> _logger;
    private readonly JwtTokenGenerator _tokenGenerator = new();

    public UsersController(TwitPosterContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegistrationRequest request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if(existingUser != null)
        {
            throw new Exception("Unhandled exception: user already exists");
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
        return Ok(new RegistrationResponse(user.Id, accessToken));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return BadRequest();
        }
        
        if (user.Password != request.Password)
        {
            return BadRequest();
        }
        
        var accessToken = _tokenGenerator.GenerateToken(user);
        return Ok(new RegistrationResponse(user.Id, accessToken));
    }
}