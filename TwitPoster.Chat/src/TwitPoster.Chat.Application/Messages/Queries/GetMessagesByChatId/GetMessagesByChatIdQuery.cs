using MediatR;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat.Application.Messages.Queries.GetMessagesByChatId;

public record GetMessagesByChatIdQuery(string ChatId) : IRequest<IReadOnlyList<Message>>;