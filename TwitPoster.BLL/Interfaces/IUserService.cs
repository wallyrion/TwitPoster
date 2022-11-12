using LanguageExt.Common;
using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Interfaces;

public interface IUsersService
{
    Task<(int UserId, string AccessToken)> Login(string email, string password);
    Task<Result<(int UserId, string AccessToken)>> Register(string firstName, string lastName, DateTime birthDate, string email, string password);
    Task Ban(int userId);
    Task Subscribe(int userId);
    Task Unsubscribe(int userId);
    Task<List<UserSubscription>> GetSubscriptions();
    Task<List<UserSubscription>> GetSubscribers();
    Task<AccountDetailDto> GetCurrentAccountDetail();
}