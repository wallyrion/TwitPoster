using FluentValidation;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Common.Interfaces;

namespace TwitPoster.Chat.Infrastructure.Auth;

public sealed class AuthOptions : ITwitposterOptions<AuthOptions>
{
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
    
    public required string Secret { get; init; }
    
    public required TimeSpan Expiration { get; init; }
    
    public static string SectionName => "Auth";

    public InlineValidator<AuthOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.Secret).NotEmpty(),
        rules => rules.RuleFor(x => x.Audience).NotEmpty(),
        rules => rules.RuleFor(x => x.Issuer).NotEmpty(),
        rules => rules.RuleFor(x => x.Expiration).NotEmpty(),
    };
}