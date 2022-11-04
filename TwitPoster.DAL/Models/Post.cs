namespace TwitPoster.DAL.Models;

public class Post
{
    public int Id { get; set; }
    
    public string Body { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }

    public User Author { get; set; } = null!;
    public int AuthorId { get; set; }
}