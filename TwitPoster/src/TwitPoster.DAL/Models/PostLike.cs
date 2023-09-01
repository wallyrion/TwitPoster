namespace TwitPoster.DAL.Models;

public class PostLike
{
    public Post Post { get; set; } = null!;
    public int PostId { get; set; }
    public User User { get; set; } = null!;
    public int UserId { get; set; }
}