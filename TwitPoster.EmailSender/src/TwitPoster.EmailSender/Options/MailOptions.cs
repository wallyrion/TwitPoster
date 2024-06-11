using FluentValidation;

namespace TwitPoster.EmailSender.Options;

public class MailOptions : ITwitposterOptions<MailOptions>
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string SendEmailFrom { get; set; }
    public required string AuthUserName { get; set; }
    public required string AuthPassword { get; set; }
    public static string SectionName => "Mail";
    public InlineValidator<MailOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Host).NotEmpty(),
        rules => rules.RuleFor(x => x.Port).GreaterThan(0),
        rules => rules.RuleFor(x => x.SendEmailFrom).NotEmpty(),
        rules => rules.RuleFor(x => x.AuthUserName).NotEmpty(),
        rules => rules.RuleFor(x => x.AuthPassword).NotEmpty(),
    };
}