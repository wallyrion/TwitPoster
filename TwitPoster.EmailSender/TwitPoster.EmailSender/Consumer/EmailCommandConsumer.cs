using MassTransit;
using TwitPoster.EmailSender.Contracts;
using TwitPoster.EmailSender.Services;

namespace TwitPoster.EmailSender.Consumer;

// ReSharper disable once UnusedType.Global used by MassTransit
public class EmailCommandConsumer : IConsumer<EmailCommand>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailCommandConsumer> _logger;
    
    public EmailCommandConsumer(IEmailService emailService, ILogger<EmailCommandConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmailCommand> context)
    {
        _logger.LogInformation("Consuming email command {@EmailCommand}", context.Message);
        await _emailService.SendEmail(context.Message);
    }
}