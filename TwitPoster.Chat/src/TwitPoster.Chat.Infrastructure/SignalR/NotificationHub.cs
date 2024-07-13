using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Infrastructure.SignalR;

[Authorize]
public class NotificationHub
    : Hub<INotificationClient>
{
    
    private readonly IChatsRepository _chatsRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<NotificationHub> _logger;
    
    public const string EndpointPath = "/messages";

    public NotificationHub(IChatsRepository chatsRepository, ILogger<NotificationHub> logger, IMessagesRepository messagesRepository)
    {
        _chatsRepository = chatsRepository;
        _logger = logger;
        _messagesRepository = messagesRepository;
    }
    
    public async Task Hello(SentChatMessage sentMessage)
    {
        var roomChat = await _chatsRepository.GetAsync(sentMessage.ChatId);
        
        if (roomChat is null)
        {
            return;
        }

        var authorId = int.Parse(Context.UserIdentifier!);
        
        if (!roomChat.ParticipantsIds.Contains(authorId))
        {
            return;
        }

        var message = new Message(sentMessage.Text, authorId, sentMessage.ChatId);
        await _messagesRepository.CreateAsync(message);
        
        var receivedMessage = new ReceivedChatMessage(sentMessage.ChatId, message.Text, authorId, message.Created);

        var participantIds = roomChat.ParticipantsIds.Select(p => p.ToString());
        await Clients.Users(participantIds).ReceivedMessage(receivedMessage);
    }
    
    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext is null)
        {
            _logger.LogError("Could not get HttpContext when connecting to {ConnectionId}", Context.ConnectionId);

            return Task.CompletedTask;
        }

        _logger.LogInformation("User {UserId} connected with {ConnectionId} for notifications", Context.UserIdentifier, Context.ConnectionId);

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User {UserId} disconnected with {ConnectionId} from notifications", Context.UserIdentifier, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}