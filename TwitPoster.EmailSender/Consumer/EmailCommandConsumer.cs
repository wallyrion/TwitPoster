using MassTransit;
using TwitPoster.Contracts;
using TwitPoster.EmailSender.Services;

namespace TwitPoster.EmailSender.Consumer;

// ReSharper disable once UnusedType.Global used by MassTransit
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
    }
}