using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TwitPoster.Controllers;

public static class ControllerBaseExtensions
{
    public static int GetUserId(this ControllerBase controller)
    {
        var userIdFromToken = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var id = int.Parse(userIdFromToken ?? throw new InvalidOperationException("Can not retrieve user id from token"));

        return id;
    }
}