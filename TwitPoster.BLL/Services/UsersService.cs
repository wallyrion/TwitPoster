using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Services;

public class UsersService : IUsersService
{
    private readonly JwtTokenGenerator _tokenGenerator = new();
    private readonly TwitPosterContext _context;

    public UsersService(TwitPosterContext context)
    {
        _context = context;
    }

    public async Task<(int UserId, string AccessToken)> Login(string email, string password)
    {
        var user = await _context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.UserAccount.Password != password)
        {
            throw new TwitPosterValidationException("Your password or email is incorrect");
        }
        
        var accessToken = _tokenGenerator.GenerateToken(user);
        return (user.Id, accessToken);
    }
    
    public async Task<Result<(int UserId, string AccessToken)>> Register(string firstName, string lastName, DateTime birthDate, string email, string password)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if(existingUser != null)
        {
            return new Result<(int UserId, string AccessToken)>(new TwitPosterValidationException("User with this email already exists"));
        }                           
        
        var user = new User
        {
            CreatedAt = DateTime.UtcNow,
            Email = email,
            BirthDate = birthDate,
            FirstName = firstName,
            LastName = lastName, 
            UserAccount = new UserAccount
            {
                Password = password,
                Role = UserRole.User
            }
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _tokenGenerator.GenerateToken(user);
        return (user.Id, accessToken);
    }

    public async Task Ban(int userId)
    {
        var user = await _context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new TwitPosterValidationException("User not found");
        }

        if (user.UserAccount.Role > UserRole.User)
        {
            throw new TwitPosterValidationException("Not enough rights to ban this user");
        }

        user.IsBanned = true;
        await _context.SaveChangesAsync();
    }
}