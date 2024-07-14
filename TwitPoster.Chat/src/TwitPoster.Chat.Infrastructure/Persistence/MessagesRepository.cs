using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Domain.MessageAggregateRoot;
using TwitPoster.Chat.Infrastructure.Persistence.Models;

namespace TwitPoster.Chat.Infrastructure.Persistence;

internal class MessagesRepository : IMessagesRepository
{
    private readonly IMongoCollection<MessageDbDto> _messagesCollection;

    public MessagesRepository(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _messagesCollection = mongoDatabase.GetCollection<MessageDbDto>(
            bookStoreDatabaseSettings.Value.MessagesCollectionName);
    }

    public async Task<List<Message>> GetAsync()
    {
        var result = _messagesCollection.AsQueryable();

        var query = result.Select(r => new Message
        {
            Created = r.Created,
            AuthorId = r.AuthorId,
            Text = r.Text,
            Id = r.Id,
            ChatRoomId = r.ChatRoomId
        }).ToListAsync();

        return await query;

        /*return await _messagesCollection.Find(_ => true).Project(r => new Message
        {
            Created = r.Created,
            AuthorId = r.AuthorId,
            Text = r.Text,
            Id = r.Id,
            ChatRoomId = r.ChatRoomId
        }).ToListAsync();*/
    }
    
    public async Task<Message?> GetAsync(string id) =>
        await _messagesCollection.Find(x => x.Id == id).Project(r => new Message
        {
            Created = r.Created,
            AuthorId = r.AuthorId,
            Text = r.Text,
            Id = r.Id,
            ChatRoomId = r.ChatRoomId
        }).FirstOrDefaultAsync();

    public async Task CreateAsync(Message newMessage)
    {
        var messageDbDto = new MessageDbDto
        {
            Created = newMessage.Created,
            AuthorId = newMessage.AuthorId,
            Text = newMessage.Text,
            ChatRoomId = newMessage.ChatRoomId
        };

        await _messagesCollection.InsertOneAsync(messageDbDto);

        newMessage.Id = messageDbDto.Id;
    }

    /*public async Task UpdateAsync(string id, Message updatedMessage) =>
        await _messagesCollection.ReplaceOneAsync(x => x.Id == id, updatedMessage);*/

    /*public async Task RemoveAsync(string id) =>
        await _messagesCollection.DeleteOneAsync(x => x.Id == id);*/

    public async Task<List<Message>> GetByChatIdAsync(string chatId, CancellationToken cancellationToken = default)
    {
        return await _messagesCollection.AsQueryable()
            .OrderByDescending(x => x.Created)
            .Where(x => x.ChatRoomId == chatId)
            .Select(r => new Message
            {
                Created = r.Created,
                AuthorId = r.AuthorId,
                Text = r.Text,
                Id = r.Id,
                ChatRoomId = r.ChatRoomId
            }).ToListAsync(cancellationToken);
    }
}