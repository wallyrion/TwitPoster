using MediatR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.Exceptions;

namespace TwitPoster.Chat.Application.Messages.Commands;

internal class AddMessageToChatCommandHandler(ICurrentUser currentUser, IMessagesRepository messagesRepository, IChatsRepository chatsRepository) 
    : IRequestHandler<AddMessageToChatCommand, (Message, IReadOnlyList<int>)>
{
    public async Task<(Message, IReadOnlyList<int>)> Handle(AddMessageToChatCommand request, CancellationToken cancellationToken)
    {
        var authorId = currentUser.Id;
        
        var chat = await chatsRepository.GetAsync(request.ChatId);
        
        if (chat is null)
        {
            throw new EntityNotFoundException($"Chat {request.ChatId} not found");
        }
        
        var newMessage = new Message(request.Text, authorId, request.ChatId);
        await messagesRepository.CreateAsync(newMessage);
        
        return (newMessage, chat.ParticipantsIds);
    }
}