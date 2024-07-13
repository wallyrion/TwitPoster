using MediatR;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application.Messages.Commands;

public record AddMessageToChatCommand(string ChatId, string Text) : IRequest<(Message, IReadOnlyList<int>)>;
