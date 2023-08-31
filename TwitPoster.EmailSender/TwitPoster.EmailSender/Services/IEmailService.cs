using TwitPoster.EmailSender.Contracts;

namespace TwitPoster.EmailSender.Services;

public interface IEmailService
{
    Task SendEmail(EmailCommand command);
}