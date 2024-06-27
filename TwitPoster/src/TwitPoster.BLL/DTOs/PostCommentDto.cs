namespace TwitPoster.BLL.DTOs;

public class PostCommentDto
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required AuthorDto Author { get; init; }
}