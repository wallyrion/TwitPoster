namespace TwitPoster.Chat.Application;

public interface ICurrentUser
{
    int Id { get; } 
    string Email { get; }
}