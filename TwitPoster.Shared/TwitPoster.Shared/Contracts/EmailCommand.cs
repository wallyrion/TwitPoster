namespace TwitPoster.Shared.Contracts;

public enum TextFormat
{
    /// <summary>The plain text format.</summary>
    Plain = 0,
    /// <summary>An alias for the plain text format.</summary>
    Text = 0,
    /// <summary>The flowed text format (as described in rfc3676).</summary>
    Flowed = 1,
    /// <summary>The HTML text format.</summary>
    Html = 2,
    /// <summary>The enriched text format.</summary>
    Enriched = 3,
    /// <summary>The compressed rich text format.</summary>
    CompressedRichText = 4,
    /// <summary>The rich text format.</summary>
    RichText = 5,
}
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
