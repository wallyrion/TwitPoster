using MediatR;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat.Application.Messages.Commands;

public record AddMessageToChatCommand(string ChatId, string Text, int ByUserId) : IRequest<(Message, IReadOnlyList<int>)>;
