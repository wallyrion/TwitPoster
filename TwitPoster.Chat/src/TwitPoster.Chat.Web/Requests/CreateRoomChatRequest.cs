namespace TwitPoster.Chat;

public record CreateRoomChatRequest(List<int> ParticipantsIds, string? Name = null);