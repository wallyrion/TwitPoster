namespace TwitPoster.AutoTests.Models;

public record PostCommentViewModel(
    int Id,
    string Text,
    DateTime? UpdatedAt,
    DateTime CreatedAt,
    AuthorViewModel Author
);