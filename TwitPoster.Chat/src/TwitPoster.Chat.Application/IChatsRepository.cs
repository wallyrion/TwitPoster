using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application;

public interface IChatsRepository
{
    Task<List<RoomChat>> GetAsync();
    Task<RoomChat?> GetAsync(string id);
    Task CreateAsync(RoomChat chat);
}