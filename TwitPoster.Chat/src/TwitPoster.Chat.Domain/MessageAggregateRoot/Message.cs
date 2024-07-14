namespace TwitPoster.Chat.Domain;

public class Message
{
    public required string Id { get; set; } = null!;

    public required string Text { get; init; }

    public required string ChatRoomId { get; init; }

    public required int AuthorId { get; init; }
    
    public required DateTime Created { get; init; }
}