using MassTransit;
using TwitPoster.EmailSender.Services;
using TwitPoster.Shared.Contracts;

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
        try
        {
            Console.WriteLine("Consuming email command");
            _logger.LogInformation("Consuming email command {@EmailCommand}", context.Message);
            await _emailService.SendEmail(context.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }
}