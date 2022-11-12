namespace TwitPoster.BLL.DTOs;

public class PostCommentDto
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required AuthorDto Author { get; set; }
}