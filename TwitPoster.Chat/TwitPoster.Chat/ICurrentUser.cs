namespace TwitPoster.BLL.Extensions;

public interface ICurrentUser
{
    int Id { get; } 
    string Email { get; }
}