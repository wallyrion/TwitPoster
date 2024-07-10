using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TwitPoster.Chat;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Text { get; set; }

    public int AuthorId { get; set; }

    public Message(string text, int authorId)
    {
        Text = text;
        AuthorId = authorId;
    }
}