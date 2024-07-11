
using TwitPoster.Chat.Application;

namespace TwitPoster.Chat.Infrastructure.Auth;

internal class CurrentUser : ICurrentUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
}

