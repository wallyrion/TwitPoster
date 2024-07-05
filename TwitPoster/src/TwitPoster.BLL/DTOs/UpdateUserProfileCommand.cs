namespace TwitPoster.BLL.DTOs;

public class UpdateUserProfileCommand
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateTime BirthDate { get; init; }
}