namespace TwitPoster.Web.ViewModels.Post;

public record PostCommentViewModel(
    int Id,
    string Text,
    DateTime? UpdatedAt,
    DateTime CreatedAt,
    AuthorViewModel Author
);