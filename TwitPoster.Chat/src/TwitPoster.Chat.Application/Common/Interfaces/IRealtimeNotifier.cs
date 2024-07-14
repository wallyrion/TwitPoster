using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat.Application.Common.Interfaces;

public interface IRealtimeNotifier
{
    Task NotifyNewMessageAdded(Message message, IEnumerable<int> userIds, CancellationToken ct = default);
}