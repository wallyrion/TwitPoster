namespace TwitPoster.Chat.Domain.ChatAggregateRoot;

public class RoomChat
{
    public string Id { get; set; } = null!;
    public required string? Name { get; init; }
    public required IReadOnlyList<int> ParticipantsIds { get; init; } = [];
    public required DateTime CreatedAt { get; init; }
}