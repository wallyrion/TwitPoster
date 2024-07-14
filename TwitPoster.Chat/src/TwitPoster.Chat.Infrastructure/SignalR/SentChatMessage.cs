using TwitPoster.Chat.Application.Messages.Commands;

namespace TwitPoster.Chat.Infrastructure.SignalR;

public record SentChatMessage(string ChatId, string Text);
public record ReceivedChatMessage(string ChatId, string Text, int CreatedByUserId, DateTimeOffset CreatedAt);