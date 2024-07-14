using MediatR;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.Common.Exceptions;
using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat.Application.Messages.Commands;

internal class AddMessageToChatCommandHandler(ICurrentUser currentUser, IMessagesRepository messagesRepository, IChatsRepository chatsRepository, IRealtimeNotifier notifier) 
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

        var newMessage = new Message
        {
            Created = DateTime.UtcNow,
            Id = null!,
            Text = request.Text,
            AuthorId = authorId,
            ChatRoomId = chat.Id
        };
        
        await messagesRepository.CreateAsync(newMessage);
        await notifier.NotifyNewMessageAdded(newMessage, chat.ParticipantsIds, cancellationToken);
        
        return (newMessage, chat.ParticipantsIds);
    }
}