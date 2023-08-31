namespace TwitPoster.DAL.Models;

public class PostComment
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime? UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public User Author { get; set; } = null!;
    public int AuthorId { get; set; }
    public int PostId { get; set; }
}