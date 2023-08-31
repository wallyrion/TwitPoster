namespace TwitPoster.DAL.Models;

public class Post
{
    public int Id { get; set; }
    
    public string Body { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }

    public User Author { get; set; } = null!;
    public int AuthorId { get; set; }

    public List<PostComment> Comments { get; set; } = new();
    public List<PostLike> PostLikes { get; set; } = new();
    
    public int LikesCount { get; set; }
}