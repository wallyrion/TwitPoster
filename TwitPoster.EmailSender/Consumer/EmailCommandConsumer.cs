using System.Text.Json;
using MassTransit;
using TwitPoster.Contracts;
using TwitPoster.EmailSender.Services;

namespace TwitPoster.EmailSender.Consumer;

public class EmailCommandConsumer : IConsumer<EmailCommand>
{
    private readonly IEmailService _emailService;

    
    public EmailCommandConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<EmailCommand> context)
    {
        await _emailService.SendEmail(context.Message);
        var jsonMessage = JsonSerializer.Serialize(context.Message);
        Console.WriteLine($"OrderCreated message: {jsonMessage}");
    }
}