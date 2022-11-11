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
            Role = user.UserAccount.Role
        };
    } 
}