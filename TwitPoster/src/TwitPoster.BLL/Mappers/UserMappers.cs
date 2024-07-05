using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Mappers;

public static class UserMappers
{
    public static AccountDetailDto ToAccountDetailDto(this User user)
    {
        return new AccountDetailDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            BirthDate = user.BirthDate,
            Role = user.UserAccount.Role,
            PhotoUrl = user.PhotoUrl,
            ThumbnailPhotoUrl = user.ThumbnailPhotoUrl,
        };
    }

    public static AuthorDto ToAuthorDto(this User user)
    {
        return new AuthorDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}