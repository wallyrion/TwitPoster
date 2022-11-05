namespace TwitPoster.DAL.Models;

public class UserAccount
{
    public UserRole Role { get; set; } = UserRole.User;
    public string Password { get; set; } = null!;
}