namespace TwitPoster.Web.ViewModels;

public record AuthorViewModel(
    int Id,
    string FullName,
    string Email,
    string? PhotoUrl,
    string? ThumbnailPhotoUrl
);
