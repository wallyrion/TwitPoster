namespace TwitPoster.Chat.IntegrationTests;

public class TestMessageDto
{
    public required string Id { get; init; }

    public required string Text { get; init; }
    
    public required string ChatRoomId { get; init; }   
    
    public required int AuthorId { get; init; }
    public required DateTime Created { get; init; }
}