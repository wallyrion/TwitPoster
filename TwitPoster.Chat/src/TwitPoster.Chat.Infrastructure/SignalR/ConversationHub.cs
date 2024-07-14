using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TwitPoster.Chat.Application.Messages.Events;

namespace TwitPoster.Chat.Infrastructure.SignalR;

[Authorize]
public class ConversationHub(ILogger<ConversationHub> logger, ITopicProducer<string, MessageAddedToChatEvent> topicProducer)
    : Hub<IConversationClient>
{
    public const string EndpointPath = "/messages";

    public async Task Hello(SentChatMessage sentMessage)
    {
        await topicProducer.Produce(sentMessage.ChatId, new MessageAddedToChatEvent(sentMessage.ChatId, sentMessage.Text, int.Parse(Context.UserIdentifier!)));
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