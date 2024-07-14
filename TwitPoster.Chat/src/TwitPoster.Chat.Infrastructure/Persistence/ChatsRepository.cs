using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.ChatAggregateRoot;
using TwitPoster.Chat.Infrastructure.Persistence.Models;

namespace TwitPoster.Chat.Infrastructure.Persistence;

internal class ChatsRepository : IChatsRepository
{
    private readonly IMongoCollection<ChatDbDto> _chats;

    public ChatsRepository(
        IOptions<DatabaseSettings> dbOptions)
    {
        var mongoClient = new MongoClient(
            dbOptions.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbOptions.Value.DatabaseName);

        _chats = mongoDatabase.GetCollection<ChatDbDto>(
            dbOptions.Value.ChatsCollectionName);
    }

    public async Task<List<RoomChat>> GetAsync(int userId) =>
        await _chats.AsQueryable()
            .Where(x => x.ParticipantsIds.Contains(userId))
            .Select(x => new RoomChat
        {
            Id = x.Id,
            Name = x.Name,
            CreatedAt = x.CreatedAt,
            ParticipantsIds = x.ParticipantsIds
        }).ToListAsync();

    public async Task<RoomChat?> GetAsync(string id)
    {
        var r= await _chats.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (r is null)
        {
            return null;
        }

        return new RoomChat
        {
            Id = r.Id,
            Name = r.Name,
            CreatedAt = r.CreatedAt,
            ParticipantsIds = r.ParticipantsIds
        };
    }

    public async Task CreateAsync(RoomChat chat)
    {
        var dbDto = new ChatDbDto
        {
            Id = chat.Id,
            Name = chat.Name,
            CreatedAt = chat.CreatedAt,
            ParticipantsIds = chat.ParticipantsIds
        };

        await _chats.InsertOneAsync(dbDto);

        chat.Id = dbDto.Id;
    }
}