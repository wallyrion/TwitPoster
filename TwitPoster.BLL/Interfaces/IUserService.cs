using LanguageExt;
using LanguageExt.Common;
using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IUsersService
{
    Task<string> Login(string email, string password);
    Task<Result<int>> Register(string firstName, string lastName, DateTime birthDate, string email, string password);
    Task Ban(int userId);
    Task Subscribe(int userId);
    Task Unsubscribe(int userId);
    Task<List<UserSubscriptionDto>> GetSubscriptions();
    Task<List<UserSubscriptionDto>> GetSubscribers();
    Task<AccountDetailDto> GetCurrentAccountDetail();
}