using LanguageExt;
using LanguageExt.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MimeKit.Text;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.DTOs;
using TwitPoster.BLL.Exceptions;
using TwitPoster.BLL.Extensions;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Mappers;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Services;

public class UserService : IUsersService
{
    private readonly JwtTokenGenerator _tokenGenerator = new();
    private readonly TwitPosterContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IEmailSender _emailSender;
    
    public UserService(TwitPosterContext context, ICurrentUser currentUser, IEmailSender emailSender)
    {
        _context = context;
        _currentUser = currentUser;
        _emailSender = emailSender;
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


        user.UserAccount.IsBanned = true;
        await _context.SaveChangesAsync();
    }

    public async Task Subscribe(int userId)
    {
        var userToFollow = await _context.Users.FindAsync(userId);
        
        if (userToFollow == null)
        {
            throw new TwitPosterValidationException("User not found");
        }
        
        var isAlreadySubscribed = await _context.UserSubscriptions.AnyAsync(s => s.SubscriberId == _currentUser.Id && s.SubscriptionId == userId);
        
        if (isAlreadySubscribed)
        {
            return;
        }

        await _context.UserSubscriptions.AddAsync(new UserSubscription
        {
            SubscriberId = _currentUser.Id,
            SubscriptionId = userId,
            SubscribedAt = DateTime.UtcNow
        });
            
        await _context.SaveChangesAsync();
    }

    public async Task Unsubscribe(int userId)
    {
        var subscriptionToUnsubscribe = await _context.UserSubscriptions.FirstOrDefaultAsync(s => s.SubscriberId == _currentUser.Id && s.SubscriptionId == userId);
        
        if (subscriptionToUnsubscribe != null)
        {
            _context.UserSubscriptions.Remove(subscriptionToUnsubscribe);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserSubscriptionDto>> GetSubscriptions()
    {
        TypeAdapterHelper.Override<UserSubscription, UserSubscriptionDto>(out var config)
            .Map(dest => dest.User, src => src.Subscription);

        return await _context.UserSubscriptions
            .Include(u => u.Subscription)
            .Where(s => s.SubscriberId == _currentUser.Id)
            .ProjectToType<UserSubscriptionDto>(config)
            .ToListAsync();
    }

    public async Task<List<UserSubscriptionDto>> GetSubscribers()
    {
        TypeAdapterHelper.Override<UserSubscription, UserSubscriptionDto>(out var config)
            .Map(dest => dest.User, src => src.Subscriber);
        
        return await _context
            .UserSubscriptions.Include(u => u.Subscriber)
            .Where(s => s.SubscriptionId == _currentUser.Id)
            .ProjectToType<UserSubscriptionDto>(config)
            .ToListAsync();
    }

    public async Task<AccountDetailDto> GetCurrentAccountDetail()
    {
        var currentUser = await _context.Users.Include(u => u.UserAccount).FirstAsync(u => u.Id == _currentUser.Id);

        return currentUser.ToAccountDetailDto();
    }
}