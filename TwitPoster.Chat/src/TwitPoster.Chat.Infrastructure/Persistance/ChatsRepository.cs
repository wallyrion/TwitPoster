using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat.Infrastructure.Persistance;

internal class ChatsRepository : IChatsRepository
{
    private readonly IMongoCollection<RoomChat> _chats;

    public ChatsRepository(
        IOptions<DatabaseSettings> dbOptions)
    {
        var mongoClient = new MongoClient(
            dbOptions.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbOptions.Value.DatabaseName);

        _chats = mongoDatabase.GetCollection<RoomChat>(
            dbOptions.Value.ChatsCollectionName);
    }

    public async Task<List<RoomChat>> GetAsync() =>
        await _chats.Find(_ => true).ToListAsync();

    public async Task<RoomChat?> GetAsync(string id) =>
        await _chats.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(RoomChat newMessage) =>
        await _chats.InsertOneAsync(newMessage);
}