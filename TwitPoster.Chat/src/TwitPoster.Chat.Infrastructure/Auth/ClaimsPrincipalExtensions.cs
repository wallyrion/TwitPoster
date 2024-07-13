using System.Security.Claims;
using TwitPoster.Chat.Application.Common.Interfaces;

namespace TwitPoster.Chat.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static void AddClaimsToCurrentUser(this ClaimsPrincipal? user, ICurrentUser currentUser)
    {
        if (user is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        var userIdFromToken = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var id = int.Parse(userIdFromToken ?? throw new InvalidOperationException("Can not retrieve user id from token"));
        var email = user.FindFirstValue(ClaimTypes.Email);

        var currentUserEditable = currentUser as CurrentUser;
        currentUserEditable!.Id = id;
        currentUserEditable.Email = email!;
    }
}