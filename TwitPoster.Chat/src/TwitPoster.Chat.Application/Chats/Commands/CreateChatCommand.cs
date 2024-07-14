using MediatR;
using TwitPoster.Chat.Domain.ChatAggregateRoot;

namespace TwitPoster.Chat.Application.Chats.Commands;

public record CreateChatCommand(IReadOnlyList<int> ParticipantsIds, string? Name = null) : IRequest<RoomChat>;
