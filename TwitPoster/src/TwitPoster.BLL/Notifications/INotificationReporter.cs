namespace TwitPoster.BLL.Notifications;

public interface INotificationReporter
{
    Task NotifyPostWasLikedAsync(int userId, NotificationPayload payload, CancellationToken ct = default);
}