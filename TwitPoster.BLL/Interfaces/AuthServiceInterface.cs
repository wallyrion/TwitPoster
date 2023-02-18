using LanguageExt.Common;
using TwitPoster.BLL.Exceptions;

namespace TwitPoster.BLL.Interfaces;

public interface AuthServiceInterface
{
    /// <summary>
    /// Tries to login user with given credentials.
    /// </summary>
    /// <exception cref="TwitPosterValidationException">Validation exception is thrown when:
    /// <list type="bullet"><item>Password or email is incorrect</item> <item>Email is not confirmed</item></list>
    /// </exception>
    /// <param name="email">User's email</param>
    /// <param name="password">User's password</param>
    /// <returns>Generated access token </returns>
    Task<string> Login(string email, string password);
    Task<Result<int>> Register(string firstName, string lastName, DateTime birthDate, string email, string password);
    Task ConfirmEmail(Guid token);
}