namespace TwitPoster.DAL.Models;

public class UserAccount
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public string Password { get; set; } = null!;
    public bool IsBanned { get; set; }
}