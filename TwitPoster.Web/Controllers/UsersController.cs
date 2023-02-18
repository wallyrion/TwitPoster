using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL.Models;
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

    [HttpGet("me")]
    public async Task<ActionResult<AccountDetailDto>> GetCurrentUser()
    {
        return await _usersService.GetCurrentAccountDetail();
    }

    [HttpPut("ban/{userId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task Ban(int userId)
    {
        await _usersService.Ban(userId);
    }

    [HttpPut("unsubscribe/{userId:int}")]
    public void Unsubscribe(int userId)
    {
        _usersService.UnsubscribeAsync(userId);
    }

    [HttpGet("subscriptions")]
    public async Task<List<UserSubscriptionViewModel>> GetSubscriptions()
    {
        var subscriptions = await _usersService.GetSubscriptions();

        Test(out var hey);

        return subscriptions.Adapt<List<UserSubscriptionViewModel>>();
    }

    private void Test(out int test)
    {
        test = 123;
    }

    [HttpPut("subscribe/{userId:int}")]
    public async Task Subscribe(int userId)
    {
        var id = userId;

        await _usersService.Subscribe(id);
    }

    [HttpGet("subscribers")]
    public async Task<List<UserSubscriptionViewModel>> GetSubscribers()
    {
        var subscriptions = await _usersService.GetSubscribers();

        return subscriptions.Adapt<List<UserSubscriptionViewModel>>();
    }
}