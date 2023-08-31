namespace TwitPoster.Web.ViewModels;

public record RegistrationRequest(string FirstName, string LastName, DateTime BirthDate, string Email, string Password);
