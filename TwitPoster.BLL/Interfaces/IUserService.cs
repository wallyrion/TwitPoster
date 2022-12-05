using LanguageExt.Common;
using TwitPoster.BLL.DTOs;

namespace TwitPoster.BLL.Interfaces;

public interface IUsersService
{
    Task Ban(int userId);
    Task Subscribe(int userId);
    Task Unsubscribe(int userId);
    Task<List<UserSubscriptionDto>> GetSubscriptions();
    Task<List<UserSubscriptionDto>> GetSubscribers();
    Task<AccountDetailDto> GetCurrentAccountDetail();
}