using MassTransit;
using TwitPoster.EmailSender.Services;
using TwitPoster.Shared.Contracts;

namespace TwitPoster.EmailSender.Consumer;

// ReSharper disable once UnusedType.Global used by MassTransit
// ReSharper disable once ClassNeverInstantiated.Global
public class EmailCommandConsumer(IEmailService emailService, ILogger<EmailCommandConsumer> logger) : IConsumer<EmailCommand>
{
    public async Task Consume(ConsumeContext<EmailCommand> context)
    {
        try
        {
            logger.LogInformation("Consuming email command {@EmailCommand}", context.Message);
            await emailService.SendEmail(context.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }
}