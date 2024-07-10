
using TwitPoster.BLL.Extensions;

namespace TwitPoster.Chat;

internal class CurrentUser : ICurrentUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
}

