namespace TwitPoster.Chat.Domain;

public class Message
{
    public string Id { get; set; }

    public  string Text { get; init; }
    
    public  string ChatRoomId { get; init; }   
    
    public  int AuthorId { get; init; }
    
    public  DateTime Created { get; init; }

    public Message()
    {
        
    }
    
    public Message(string text, int authorId, string chatId)
    {
        Text = text;
        AuthorId = authorId;
        ChatRoomId = chatId;
        Created = DateTime.UtcNow;
    }
}