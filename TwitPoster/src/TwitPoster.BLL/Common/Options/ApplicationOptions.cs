using FluentValidation;

namespace TwitPoster.BLL.Common.Options;

public sealed class ApplicationOptions : ITwitposterOptions<ApplicationOptions>
{
    public required string TwitPosterUrl { get; init; }
    public required string GoogleClientId { get; init; }
    
    public static string SectionName => "Application";

    public InlineValidator<ApplicationOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.TwitPosterUrl).NotEmpty(),
        rules => rules.RuleFor(x => x.GoogleClientId).NotEmpty(),
    };
}