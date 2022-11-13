using TwitPoster.BLL.Interfaces;

namespace TwitPoster.Web;

internal class CurrentUser : ICurrentUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
}