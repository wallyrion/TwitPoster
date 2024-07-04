using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TwitPoster.Web.Notifications;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger)
    : Hub<INotificationClient>
{
    public const string EndpointPath = "/notifications";

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext is null)
        {
            logger.LogError("Could not get HttpContext when connecting to {ConnectionId}", Context.ConnectionId);

            return Task.CompletedTask;
        }

        logger.LogInformation("User {UserId} connected with {ConnectionId} for notifications", Context.UserIdentifier, Context.ConnectionId);

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("User {UserId} disconnected with {ConnectionId} from notifications", Context.UserIdentifier, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}