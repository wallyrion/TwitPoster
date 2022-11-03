namespace TwitPoster.ViewModels;

public record CreateUserRequest(string FirstName, string LastName, DateTime BirthDate, string Email);
