using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TwitPoster.Chat.Domain;

public class RoomChat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("participants")]
    public List<Guid> Participants { get; set; } = [];
    
    [BsonElement("createdAt")]

    public DateTime CreatedAt { get; set; }
}