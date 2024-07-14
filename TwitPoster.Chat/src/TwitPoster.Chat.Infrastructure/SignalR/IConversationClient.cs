
namespace TwitPoster.Chat.Infrastructure.SignalR;

/// <summary>
/// Documents the interface the notification clients that use SignalR must implement.
/// </summary>
public interface IConversationClient
{
    Task ReceivedMessage(ReceivedChatMessage message, CancellationToken ct = default);
}