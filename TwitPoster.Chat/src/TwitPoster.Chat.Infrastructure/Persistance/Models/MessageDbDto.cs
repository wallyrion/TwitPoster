using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TwitPoster.Chat.Domain;

public class MessageDbDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("text")]
    public string Text { get; set; }
    
    [BsonElement("chatRoomId")]
    public string ChatRoomId { get; set; }   
    
    [BsonElement("authorId")]
    public int AuthorId { get; set; }
    
    [BsonElement("created")]
    public DateTime Created { get; set; }
}