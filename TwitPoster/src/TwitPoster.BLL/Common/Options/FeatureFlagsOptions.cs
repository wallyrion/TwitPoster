using FluentValidation;

namespace TwitPoster.BLL.Common.Options;

public sealed class FeatureFlagsOptions : ITwitposterOptions<FeatureFlagsOptions>
{
    public bool UseRabbitMq { get; init; }
    public bool UseDistributedCache { get; init; }
    
    public static string SectionName => "FeatureManagement";

    public InlineValidator<FeatureFlagsOptions> Validator => new();
}