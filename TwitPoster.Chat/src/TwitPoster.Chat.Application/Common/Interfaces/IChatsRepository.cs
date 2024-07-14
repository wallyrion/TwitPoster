using TwitPoster.Chat.Domain.ChatAggregateRoot;

namespace TwitPoster.Chat.Application.Common.Interfaces;

public interface IChatsRepository
{
    Task<List<RoomChat>> GetAsync(int userId);
    Task<RoomChat?> GetAsync(string id);
    Task CreateAsync(RoomChat chat);
}