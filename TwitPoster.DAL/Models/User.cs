namespace TwitPoster.DAL.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = null!;
    public bool IsBanned { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
}