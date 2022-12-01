using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace TwitPoster.Web.Extensions;

public static class ControllerBaseExtensions
{
    public static ActionResult ToOk<T, TResponse>(this ControllerBase controller, Result<T> result, Func<T,TResponse> successMap)
    {
        return result.Match(
            Succ: successObject => controller.Ok(successMap(successObject)),
            Fail: exception => controller.Problem(statusCode: StatusCodes.Status400BadRequest, title: exception.Message));
    }
    
    public static ActionResult ToActionResult<T, TResponse>(this ControllerBase controller, Result<T> result, Func<T, TResponse> successMap) where TResponse : ActionResult
    {
        return result.Match<ActionResult>(
            Succ: successMap,
            Fail: exception => controller.Problem(statusCode: StatusCodes.Status400BadRequest, title: exception.Message));
    }
}