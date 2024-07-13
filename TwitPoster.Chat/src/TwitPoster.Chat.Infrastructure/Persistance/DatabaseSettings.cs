using FluentValidation;
using TwitPoster.Chat.Application;

namespace TwitPoster.Chat.Infrastructure.Persistance;

public class DatabaseSettings : ITwitposterOptions<DatabaseSettings>
{
    public static string SectionName => "TwitPosterChatDb";
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
    public string MessagesCollectionName { get; init; } = "Messages";
    public string ChatsCollectionName { get; init; } = "Chats";
    
    public InlineValidator<DatabaseSettings> Validator => new()
    {
        rules => rules.RuleFor(x => x.ConnectionString).NotEmpty(),
        rules => rules.RuleFor(x => x.DatabaseName).NotEmpty(),
        rules => rules.RuleFor(x => x.MessagesCollectionName).NotEmpty(),
        rules => rules.RuleFor(x => x.ChatsCollectionName).NotEmpty(),
    };
}