using Microsoft.AspNetCore.Http;
using TwitPoster.Chat.Application.Common.Interfaces;

namespace TwitPoster.Chat.Infrastructure.Auth;

public class SetupUserClaimsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        context.User.AddClaimsToCurrentUser(currentUser);

        await next(context);
    }
}