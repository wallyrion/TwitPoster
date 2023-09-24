using FluentValidation;

namespace TwitPoster.BLL.Common.Options;

public sealed class ConnectionStringsOptions : ITwitposterOptions<ConnectionStringsOptions>
{
    public required string DbConnection { get; init; }
    public string? Redis { get; init; }
    public string? ServiceBus { get; init; }
    
    public static string SectionName => "ConnectionStrings";

    public InlineValidator<ConnectionStringsOptions> Validator => new()
    {
        rules => rules.RuleFor(x => x.DbConnection).NotEmpty()
    };
}