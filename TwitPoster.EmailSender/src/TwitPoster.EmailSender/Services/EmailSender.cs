using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TwitPoster.Shared.Contracts;
using TextFormat = MimeKit.Text.TextFormat;

namespace TwitPoster.EmailSender.Services;


public class EmailService : IEmailService
{
    private readonly MailOptions _mailOptions;

    public EmailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
    }

    public async Task SendEmail(EmailCommand command)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_mailOptions.SendEmailFrom));
        email.To.Add(MailboxAddress.Parse(command.To));
        email.Subject = command.Subject;
        var textFormat = command.Format.ToString();
        var textFormatType = Enum.Parse<TextFormat>(textFormat);
        email.Body = new TextPart(textFormatType) { Text = command.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailOptions.Host, _mailOptions.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailOptions.AuthUserName, _mailOptions.AuthPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}