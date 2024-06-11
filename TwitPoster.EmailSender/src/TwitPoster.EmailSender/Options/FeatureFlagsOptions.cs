using FluentValidation;

namespace TwitPoster.EmailSender.Options;

public sealed class FeatureFlagsOptions : ITwitposterOptions<FeatureFlagsOptions>
{
    public bool UseRabbitMq { get; init; }
    
    public static string SectionName => "FeatureManagement";

    public InlineValidator<FeatureFlagsOptions> Validator => new();
}