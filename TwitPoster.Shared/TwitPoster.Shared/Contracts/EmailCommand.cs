using MimeKit.Text;

namespace TwitPoster.Shared.Contracts;

public sealed class EmailCommand
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public TextFormat Format { get; set; }

    
    public EmailCommand(string to, string subject, string body, TextFormat format)
    {
        To = to;
        Subject = subject;
        Body = body;
        Format = format;
    }
    
}
