using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Extensions;
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
        var registerResponse = await _userService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToOk(registerResponse, result => new RegistrationResponse(result.UserId, result.AccessToken));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var loginResponse = await _userService.Login(request.Email, request.Password);
        return Ok(new RegistrationResponse(loginResponse.UserId, loginResponse.AccessToken));
    }
}