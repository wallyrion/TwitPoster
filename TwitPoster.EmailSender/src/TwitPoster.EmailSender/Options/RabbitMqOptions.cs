using FluentValidation;

namespace TwitPoster.EmailSender.Options;

public class RabbitMqOptions : ITwitposterOptions<RabbitMqOptions>
{
    public required string Host { get; set; }
    public required ushort Port { get; set; }
    public required string User { get; set; }
    public required string Pass { get; set; }
    public static string SectionName => "RabbitMQ";
    public InlineValidator<RabbitMqOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Host).NotEmpty()
    };
}