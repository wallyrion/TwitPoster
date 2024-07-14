using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TwitPoster.Chat.Application.Messages.Commands;
using TwitPoster.Chat.Application.Messages.Events;

namespace TwitPoster.Chat.Infrastructure.Kafka;

public sealed class MessageAddedToChatConsumer(ILogger<MessageAddedToChatConsumer> logger, ISender sender) : IConsumer<MessageAddedToChatEvent>
{
    public async Task Consume(ConsumeContext<MessageAddedToChatEvent> context)
    {
        logger.LogInformation("Received message: {@Message}", context.Message);

        var command = new AddMessageToChatCommand(context.Message.ChatId, context.Message.Text, context.Message.ByUserId);
        await sender.Send(command);
        
        logger.LogInformation("Processed message: {@Message}", context.Message);
    }
}