using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.DTOs;

public class AccountDetailDto
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public string? ThumbnailPhotoUrl { get; set; }
}