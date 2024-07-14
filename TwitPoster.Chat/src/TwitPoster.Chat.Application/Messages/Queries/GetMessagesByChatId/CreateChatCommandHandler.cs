using MediatR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application.Messages.Queries.GetMessagesByChatId;

public class GetMessagesByChatIdQueryHandler(IMessagesRepository messagesRepository) 
    : IRequestHandler<GetMessagesByChatIdQuery, IReadOnlyList<Message>>
{
    public async Task<IReadOnlyList<Message>> Handle(GetMessagesByChatIdQuery request, CancellationToken cancellationToken)
    {
        return await messagesRepository.GetByChatIdAsync(request.ChatId, cancellationToken);
    }
}