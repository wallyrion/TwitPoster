using LanguageExt.Common;

namespace TwitPoster.BLL.Interfaces;

public interface IAuthService
{
    Task<string> Login(string email, string password);
    Task<Result<int>> Register(string firstName, string lastName, DateTime birthDate, string email, string password);
    Task ConfirmEmail(Guid token);
}