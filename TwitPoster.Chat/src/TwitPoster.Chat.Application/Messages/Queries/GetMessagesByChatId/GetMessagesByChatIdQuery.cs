using MediatR;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application.Messages.Queries.GetMessagesByChatId;

public record GetMessagesByChatIdQuery(string ChatId) : IRequest<IReadOnlyList<Message>>;