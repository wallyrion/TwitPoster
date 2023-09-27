using FluentValidation;
using TwitPoster.BLL.Common.Options;

namespace TwitPoster.Web.Common.Options;

public class StorageOptions : ITwitposterOptions<StorageOptions>
{
    public required string Uri { get; init; }
    public required string SharedKey { get; init; }
    public required string AccountName { get; init; }
    public required string ContainerName { get; init; }

    public static string SectionName => "Storage";

    public InlineValidator<StorageOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Uri).NotEmpty(),
        rules => rules.RuleFor(x => x.SharedKey).NotEmpty(),
        rules => rules.RuleFor(x => x.AccountName).NotEmpty()
    };
}