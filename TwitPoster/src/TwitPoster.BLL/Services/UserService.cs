using Mapster;
using Microsoft.EntityFrameworkCore;
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
    private readonly TwitPosterContext _context;
    private readonly ICurrentUser _currentUser;

    public UserService(TwitPosterContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
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

    public async Task UnsubscribeAsync(int userId)
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