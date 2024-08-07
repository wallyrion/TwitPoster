﻿using TwitPoster.BLL.DTOs;

namespace TwitPoster.BLL.Interfaces;

public interface IUsersService
{
    Task UpdateUserProfile(UpdateUserProfileCommand command);
    Task Ban(int userId);
    Task Subscribe(int userId);
    Task UnsubscribeAsync(int userId);
    Task<List<UserSubscriptionDto>> GetSubscriptions();
    Task<List<UserSubscriptionDto>> GetSubscribers();
    Task<AccountDetailDto> GetCurrentAccountDetail();
}