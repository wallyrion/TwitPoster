using System.Security.Claims;
using TwitPoster.BLL.Extensions;
using TwitPoster.Chat;

namespace TwitPoster.Web.Middlewares;

public class SetupUserClaimsMiddleware
{
    private readonly RequestDelegate _next;
    
    public SetupUserClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == false)
        {
            await _next(context);
        }
        else
        {
            var userIdFromToken = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = int.Parse(userIdFromToken ?? throw new InvalidOperationException("Can not retrieve user id from token"));
            var email = user.FindFirstValue(ClaimTypes.Email);


            var currentUserEditable = currentUser as CurrentUser;
            currentUserEditable!.Id = id;
            currentUserEditable.Email = email!;
        
            await _next(context);
        }
    }
}