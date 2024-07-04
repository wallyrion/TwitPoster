using TwitPoster.BLL.Notifications;

namespace TwitPoster.Web.Notifications;

/// <summary>
/// Documents the interface the notification clients that use SignalR must implement.
/// </summary>
public interface INotificationClient
{
    Task SendNotification(NotificationType type, NotificationPayload payload, CancellationToken ct = default);
}