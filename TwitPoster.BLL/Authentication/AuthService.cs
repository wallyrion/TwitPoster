using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Authentication;

public class AuthService : IAuthService
{
    private readonly IEmailSender _emailSender;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly TwitPosterContext _context;

    public AuthService(IEmailSender emailSender, IJwtTokenGenerator tokenGenerator, TwitPosterContext context)
    {
        _emailSender = emailSender;
        _tokenGenerator = tokenGenerator;
        _context = context;
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.UserAccount.Password != password)
        {
            throw new TwitPosterValidationException("Your password or email is incorrect");
        }

        if (!user.UserAccount.IsEmailConfirmed)
        {
            throw new TwitPosterValidationException("Your email is not confirmed. Please follow email instructions");
        }
        
        var accessToken = _tokenGenerator.GenerateToken(user);
        return accessToken;
    }
    
    public async Task<Result<int>> Register(string firstName, string lastName, DateTime birthDate, string email, string password)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if(existingUser != null)
        {
            return new Result<int>(new TwitPosterValidationException("User with this email already exists"));
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
                Role = UserRole.User,
                EmailConfirmationToken = Guid.NewGuid()
            }
        };
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync();

        var emailBody = $"""
            <h1> You are on the way! </h1>
            <h2> Please confirm your email by clicking on the link below </h2>
            <a href="https://localhost:7267/Auth/EmailConfirmation?Token={ user.UserAccount.EmailConfirmationToken}">Press to confirm email address</a>
        """ ;

        var mailCommand = new EmailCommand(email, "Welcome to TwitPoster! Confirm your email", emailBody, TextFormat.Html);
        await _emailSender.SendEmail(mailCommand);
        return (user.Id);
    }

    public async Task ConfirmEmail(Guid token)
    {
        var user = await _context.Users.Include(u => u.UserAccount).FirstOrDefaultAsync(u => u.UserAccount.EmailConfirmationToken == token);
        
        if (user == null)
        {
            throw new TwitPosterValidationException("Not valid token");
        }
        
        user.UserAccount.IsEmailConfirmed = true;
        user.UserAccount.EmailConfirmationToken = default;
        await _context.SaveChangesAsync();
    }
}