using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;

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

    [HttpGet("me")]
    public async Task<ActionResult<AccountDetailDto>> GetCurrentUser()
    {
        return await _usersService.GetCurrentAccountDetail();
    }

    [HttpPut("ban/{userId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task Ban (int userId)
    {
        await _usersService.Ban(userId);
    }
    
    [HttpPut("subscribe/{userId:int}")]
    public async Task Subscribe(int userId)
    {
        await _usersService.Subscribe(userId);
    }
    
    [HttpPut("unsubscribe/{userId:int}")]
    public async Task Unsubscribe(int userId)
    {
        await _usersService.Unsubscribe(userId);
    }
    
    [HttpGet("subscriptions")]
    public async Task<List<UserSubscription>> GetSubscriptions()
    {
        return await _usersService.GetSubscriptions();
    }
    
    [HttpGet("subscribers")]
    public async Task<List<UserSubscription>> GetSubscribers()
    {
        return await _usersService.GetSubscribers();
    }
}