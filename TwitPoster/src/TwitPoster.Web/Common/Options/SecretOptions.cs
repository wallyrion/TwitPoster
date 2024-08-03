using FluentValidation;
using TwitPoster.BLL.Common.Options;

namespace TwitPoster.Web.Common.Options;

public class SecretOptions : ITwitposterOptions<SecretOptions>
{
    public required string KeyVaultUri { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? TenantId { get; init; }
    public required bool UseSecrets { get; init; }
    public bool UseDefaultAzureCredential { get; init; } = true;

    public static string SectionName => "Secrets";

    public InlineValidator<SecretOptions> Validator => new()
    {
        /*rules => rules.RuleFor(x => x.ClientSecret).NotEmpty().When(x => x.UseSecrets),
        rules => rules.RuleFor(x => x.ClientId).NotEmpty().When(x => x.UseSecrets),
        rules => rules.RuleFor(x => x.TenantId).NotEmpty().When(x => x.UseSecrets),
        rules => rules.RuleFor(x => x.KeyVaultUri).NotEmpty().When(x => x.UseSecrets),*/
    };
}