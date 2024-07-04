namespace TwitPoster.BLL.Interfaces;

public interface ICurrentUser
{
    int Id { get; } 
    string Email { get; }
}
