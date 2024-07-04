using Microsoft.AspNetCore.SignalR;
using TwitPoster.BLL.Notifications;

namespace TwitPoster.Web.Notifications;

public sealed class HubNotificationsReporter(IHubContext<NotificationHub, INotificationClient> hubContext) : INotificationReporter
{
    public async Task NotifyPostWasLikedAsync(int userId, NotificationPayload payload, CancellationToken ct = default)
    {
        await hubContext.Clients.User(userId.ToString()).SendNotification(NotificationType.LikedPost, payload, ct);
    }
}