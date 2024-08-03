using FluentValidation;
using TwitPoster.BLL.Common.Options;

namespace TwitPoster.Web.AI.TagsGeneration;

public class AiOptions : ITwitposterOptions<AiOptions>
{
    public required string OpenApiKey { get; init; }
    public required string Model { get; init; }

    public static string SectionName => "AI";

    public InlineValidator<AiOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.OpenApiKey).NotEmpty(),
        rules => rules.RuleFor(x => x.Model).NotEmpty()
    };
}