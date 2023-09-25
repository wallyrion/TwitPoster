using FluentValidation;
using TwitPoster.BLL.Common.Options;

namespace TwitPoster.Web.Common.Options;

public class CountriesApiOptions : ITwitposterOptions<CountriesApiOptions>
{
    public required string Uri { get; init; }

    public static string SectionName => "CountriesApi";

    public InlineValidator<CountriesApiOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Uri).NotEmpty()
    };
}