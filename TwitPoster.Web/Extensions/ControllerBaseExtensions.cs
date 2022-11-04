using System.Security.Claims;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace TwitPoster.Web.Extensions;

public static class ControllerBaseExtensions
{
    public static int GetUserId(this ControllerBase controller)
    {
        var userIdFromToken = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var id = int.Parse(userIdFromToken ?? throw new InvalidOperationException("Can not retrieve user id from token"));

        return id;
    }
    
    public static ActionResult ToOk<T, TResponse>(this ControllerBase controller, Result<T> result, Func<T,TResponse> successMap)
    {
        return result.Match(
            Succ: successObject => controller.Ok(successMap(successObject)),
            Fail: exception => controller.Problem(statusCode: StatusCodes.Status400BadRequest, title: exception.Message));
    }
}