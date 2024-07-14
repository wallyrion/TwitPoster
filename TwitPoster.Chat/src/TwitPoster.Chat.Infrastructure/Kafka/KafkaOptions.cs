using FluentValidation;
using TwitPoster.Chat.Application.Common.Interfaces;

namespace TwitPoster.Chat.Infrastructure.Kafka;

public class KafkaOptions : ITwitposterOptions<KafkaOptions>
{
    public static string SectionName => "Kafka";
    
    public required string Host { get; init; }
    public required string Topic { get; init; }
    
    public InlineValidator<KafkaOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Host).NotEmpty(),
        rules => rules.RuleFor(x => x.Topic).NotEmpty(),
    };
}