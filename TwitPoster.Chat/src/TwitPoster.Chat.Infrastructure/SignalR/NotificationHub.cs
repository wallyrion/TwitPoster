using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Application.Messages.Commands;

namespace TwitPoster.Chat.Infrastructure.SignalR;

[Authorize]
public class NotificationHub(IChatsRepository chatsRepository, ILogger<NotificationHub> logger, IMessagesRepository messagesRepository, ISender sender)
    : Hub<INotificationClient>
{
    public const string EndpointPath = "/messages";

    public async Task Hello(SentChatMessage sentMessage)
    {
        var roomChat = await chatsRepository.GetAsync(sentMessage.ChatId);
        
        if (roomChat is null)
        {
            return;
        }

        var (message, participantsIds) = await sender.Send(new AddMessageToChatCommand(sentMessage.ChatId, sentMessage.Text));
        
        var receivedMessage = new ReceivedChatMessage(sentMessage.ChatId, message.Text, message.AuthorId, message.Created);
        await Clients.Users(participantsIds.Select(x => x.ToString())).ReceivedMessage(receivedMessage);
    }
    
    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext is null)
        {
            logger.LogError("Could not get HttpContext when connecting to {ConnectionId}", Context.ConnectionId);

            return Task.CompletedTask;
        }

        logger.LogInformation("User {UserId} connected with {ConnectionId} for notifications", Context.UserIdentifier, Context.ConnectionId);

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("User {UserId} disconnected with {ConnectionId} from notifications", Context.UserIdentifier, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}