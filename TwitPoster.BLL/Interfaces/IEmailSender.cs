using TwitPoster.BLL.Services;

namespace TwitPoster.BLL.Interfaces;

public interface IEmailSender
{
    Task SendEmail(EmailCommand command);
}