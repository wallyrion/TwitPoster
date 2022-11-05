using LanguageExt.Common;

namespace TwitPoster.BLL.Interfaces;

public interface IUsersService
{
    Task<(int UserId, string AccessToken)> Login(string email, string password);
    Task<Result<(int UserId, string AccessToken)>> Register(string firstName, string lastName, DateTime birthDate, string email, string password);
}