namespace TwitPoster.BLL.DTOs;

public class AuthorDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhotoUrl { get; set; }
    public string? ThumbnailPhotoUrl { get; set; }
}