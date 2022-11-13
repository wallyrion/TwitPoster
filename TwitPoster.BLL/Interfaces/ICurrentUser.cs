namespace TwitPoster.BLL.Interfaces;

public interface ICurrentUser
{
    int Id { get; set; } 
    string Email { get; set; }
}
