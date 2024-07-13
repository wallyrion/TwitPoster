namespace TwitPoster.Chat.IntegrationTests;

public class TestMessageDto
{
    public string Id { get; set; } = null!;

    public string Text { get; set; }
    
    public string ChatRoomId { get; set; }   
    
    public int AuthorId { get; set; }

    public DateTime Created { get; set; }
}