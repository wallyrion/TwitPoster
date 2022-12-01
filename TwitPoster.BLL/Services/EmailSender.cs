using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using TwitPoster.BLL.Interfaces;

namespace TwitPoster.BLL.Services;

public record EmailCommand(string To, string Subject, string Body, TextFormat Format = TextFormat.Plain);

public class EmailSender : IEmailSender
{
    public async Task SendEmail(EmailCommand command)
    {
        // create email message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("no-reply@twitposter.com"));
        email.To.Add(MailboxAddress.Parse(command.To));
        email.Subject = command.Subject;
        email.Body = new TextPart(command.Format) { Text = command.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("omari34@ethereal.email", "KVNe9abRCYTQ6wgNS2");
        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}