using MediatR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application.Chats.Commands;

public class CreateChatCommandHandler(IChatsRepository chatsRepository) : IRequestHandler<CreateChatCommand, RoomChat>
{
    
    public async Task<RoomChat> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new RoomChat()
        {
            Name = request.Name,
            ParticipantsIds = request.ParticipantsIds,
            CreatedAt = DateTime.UtcNow,
        };
        
        await chatsRepository.CreateAsync(chat);
        
        return chat;
    }
}