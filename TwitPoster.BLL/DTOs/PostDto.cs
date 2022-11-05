namespace TwitPoster.BLL.DTOs;

public record PostDto(int Id, string Body, DateTime CreatedAt, string AuthorFirstName, string AuthorLastName, int AuthorId);