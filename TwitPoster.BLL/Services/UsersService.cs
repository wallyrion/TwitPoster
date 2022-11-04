using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Exceptions;
using TwitPoster.DAL;

namespace TwitPoster.BLL.Services;

public class UserService
{
    private readonly JwtTokenGenerator _tokenGenerator = new();
    private readonly TwitPosterContext _context;

    public UserService(TwitPosterContext context)
    {
        _context = context;
    }

    public async Task<(int UserId, string AccessToken)> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.Password != password)
        {
            throw new TwitPosterValidationException("Your password or email is incorrect");
        }
        
        var accessToken = _tokenGenerator.GenerateToken(user);
        return (user.Id, accessToken);
    }
}