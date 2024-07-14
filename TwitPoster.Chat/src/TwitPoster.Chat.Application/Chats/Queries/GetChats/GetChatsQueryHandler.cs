using MediatR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain.ChatAggregateRoot;

namespace TwitPoster.Chat.Application.Chats.Queries.GetChats;

internal class GetChatsQueryHandler(IChatsRepository chatsRepository, ICurrentUser currentUser) : IRequestHandler<GetChatsQuery, List<RoomChat>>
{
    public async Task<List<RoomChat>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var result = await chatsRepository.GetAsync(currentUser.Id);

        return result;
    }
}