namespace TwitPoster.BLL.Notifications;

public record NotificationPayload
{
    public required string ByUserName { get; init; }
}