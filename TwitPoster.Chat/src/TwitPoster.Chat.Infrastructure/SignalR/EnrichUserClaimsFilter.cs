using Microsoft.AspNetCore.SignalR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Infrastructure.Auth;

namespace TwitPoster.Chat.Infrastructure.SignalR;

public class EnrichUserClaimsFilter(ICurrentUser currentUser) : IHubFilter
{
    public ValueTask<object?> InvokeMethodAsync(HubInvocationContext context, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        context.Context.User.AddClaimsToCurrentUser(currentUser);

        return next(context);
    }
}