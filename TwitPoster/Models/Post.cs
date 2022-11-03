namespace TwitPoster;

public class Post
{
    public int Id { get; set; }
    public string Body { get; set; } = null!;
    public string Author { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}