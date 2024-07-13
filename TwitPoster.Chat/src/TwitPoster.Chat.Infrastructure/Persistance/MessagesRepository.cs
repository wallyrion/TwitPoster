using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Infrastructure.Persistance;

internal class MessagesRepository : IMessagesRepository
{
    private readonly IMongoCollection<Message> _messagesCollection;

    public MessagesRepository(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _messagesCollection = mongoDatabase.GetCollection<Message>(
            bookStoreDatabaseSettings.Value.MessagesCollectionName);
    }

    public async Task<List<Message>> GetAsync() =>
        await _messagesCollection.Find(_ => true).ToListAsync();

    public async Task<Message?> GetAsync(string id) =>
        await _messagesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Message newMessage) =>
        await _messagesCollection.InsertOneAsync(newMessage);

    public async Task UpdateAsync(string id, Message updatedMessage) =>
        await _messagesCollection.ReplaceOneAsync(x => x.Id == id, updatedMessage);

    public async Task RemoveAsync(string id) =>
        await _messagesCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<List<Message>> GetByChatIdAsync(string chatId)
    {
        // sort by descenging order
        
        var sort = Builders<Message>.Sort.Descending(x => x.Created);
        var filter = Builders<Message>.Filter.Eq(x => x.ChatRoomId, chatId);
        var result = await _messagesCollection.Find(filter).Sort(sort).ToListAsync();
        return result;
    }
}