namespace TwitPoster.Web.Middlewares;

internal class CurrentUser : ICurrentUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
}