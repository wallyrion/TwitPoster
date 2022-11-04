using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Services;
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
    private readonly UserService _userService;
    
    public UsersController(TwitPosterContext context, ILogger<UsersController> logger, UserService userService)
    {
        _context = context;
        _logger = logger;
        _userService = userService;
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
        var loginResponse = await _userService.Login(request.Email, request.Password);
        return Ok(new RegistrationResponse(loginResponse.UserId, loginResponse.AccessToken));
    }
}