using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Application.Common.Interfaces;

public interface IMessagesRepository
{
    Task<List<Message>> GetAsync();
    Task<Message?> GetAsync(string id);
    Task CreateAsync(Message newMessage);
    //Task UpdateAsync(string id, Message updatedMessage);
    //Task RemoveAsync(string id);
    Task<List<Message>> GetByChatIdAsync(string chatId, CancellationToken cancellationToken);
}