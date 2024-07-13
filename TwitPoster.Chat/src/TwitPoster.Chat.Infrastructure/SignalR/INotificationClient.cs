
namespace TwitPoster.Chat.Infrastructure.SignalR;

/// <summary>
/// Documents the interface the notification clients that use SignalR must implement.
/// </summary>
public interface INotificationClient
{
    Task ReceivedMessage(ReceivedChatMessage message);
}