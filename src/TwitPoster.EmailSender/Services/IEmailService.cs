using TwitPoster.Contracts;

namespace TwitPoster.EmailSender.Services;

public interface IEmailService
{
    Task SendEmail(EmailCommand command);
}