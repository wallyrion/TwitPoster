namespace TwitPoster.DAL.Models;

public class Post
{
    public int Id { get; set; }
    
    public string Body { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }

    public User Author { get; set; } = null!;
    public int AuthorId { get; set; }
    public int LikesCount { get; set; }
    
    public List<PostLike> Likes { get; set; } = null!;
}

public class PostLike
{
    public Post Post { get; set; } = null!;
    public int PostId { get; set; }

    public User User { get; set; } = null!;
    public int UserId { get; set; }
}