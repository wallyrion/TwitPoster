namespace TwitPoster.Web.Middlewares;

public interface ICurrentUser
{
    int Id { get; set; } 
    string Email { get; set; }
}
