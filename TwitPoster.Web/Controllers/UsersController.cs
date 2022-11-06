using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("registration")]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        var registerResponse = await _usersService.Register(request.FirstName, request.LastName, request.BirthDate, request.Email, request.Password);

        return this.ToOk(registerResponse, result => new RegistrationResponse(result.UserId, result.AccessToken));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var loginResponse = await _usersService.Login(request.Email, request.Password);
        return Ok(new RegistrationResponse(loginResponse.UserId, loginResponse.AccessToken));
    }
    
    [HttpPut("ban/{userId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task Ban (int userId)
    {
        await _usersService.Ban(userId);
    }
    
    [HttpPut("subscribe/{userId:int}")]
    [Authorize]
    public async Task Subscribe(int userId)
    {
        await _usersService.Subscribe(userId);
    }
    
    [Authorize]
    [HttpPut("unsubscribe/{userId:int}")]
    public async Task Unsubscribe(int userId)
    {
        await _usersService.Unsubscribe(userId);
    }
    
    [Authorize]
    [HttpGet("subscriptions")]
    public async Task<List<UserSubscription>> GetSubscriptions()
    {
        return await _usersService.GetSubscriptions();
    }
    
    [Authorize]
    [HttpGet("subscribers")]
    public async Task<List<UserSubscription>> GetSubscribers()
    {
        return await _usersService.GetSubscribers();
    }
}