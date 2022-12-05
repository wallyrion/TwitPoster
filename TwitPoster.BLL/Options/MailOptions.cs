namespace TwitPoster.BLL.Options;

public class MailOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string SendEmailFrom { get; set; }
    public required string AuthUserName { get; set; }
    public required string AuthPassword { get; set; }
}