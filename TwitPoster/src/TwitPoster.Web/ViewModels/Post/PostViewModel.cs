namespace TwitPoster.Web.ViewModels.Post;

public record PostViewModel(
    int Id,
    string Body,
    DateTime CreatedAt,
    string AuthorFirstName,
    string AuthorLastName,
    string AuthorPhotoUrl,
    int AuthorId,
    int LikesCount,
    bool IsLikedByCurrentUser,
    int CommentsCount
    );
