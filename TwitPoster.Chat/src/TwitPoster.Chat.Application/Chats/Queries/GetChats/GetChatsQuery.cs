using MediatR;
using TwitPoster.Chat.Domain.ChatAggregateRoot;

namespace TwitPoster.Chat.Application.Chats.Queries.GetChats;

public record GetChatsQuery : IRequest<List<RoomChat>>;