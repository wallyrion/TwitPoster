using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TwitPoster.Chat.Application;

namespace TwitPoster.Chat.Infrastructure.Auth;

public class SetupUserClaimsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        var user = context.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            var userIdFromToken = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = int.Parse(userIdFromToken ?? throw new InvalidOperationException("Can not retrieve user id from token"));
            var email = user.FindFirstValue(ClaimTypes.Email);

            var currentUserEditable = currentUser as CurrentUser;
            currentUserEditable!.Id = id;
            currentUserEditable.Email = email!;
        }

        await next(context);
    }
}