using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TwitPoster.Chat.Domain;

public class ChatDbDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("participants")]
    public IReadOnlyList<int> ParticipantsIds { get; set; } = [];
    
    [BsonElement("createdAt")]

    public DateTime CreatedAt { get; set; }
}