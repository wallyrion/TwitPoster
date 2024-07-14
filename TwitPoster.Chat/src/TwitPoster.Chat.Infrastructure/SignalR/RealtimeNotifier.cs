using Microsoft.AspNetCore.SignalR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat.Infrastructure.SignalR;

public sealed class RealtimeNotifier(IHubContext<ConversationHub, IConversationClient> hubContext) : IRealtimeNotifier
{
    public async Task NotifyNewMessageAdded(Message message, IEnumerable<int> userIds, CancellationToken ct = default)
    {
        var receivedMessage = new ReceivedChatMessage(message.ChatRoomId, message.Text, message.AuthorId, message.Created);

        await hubContext.Clients.Users(userIds.Select(x => x.ToString())).ReceivedMessage(receivedMessage, ct);
    }
}