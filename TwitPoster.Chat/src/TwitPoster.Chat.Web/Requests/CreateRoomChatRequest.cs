namespace TwitPoster.Chat.Requests;

public record CreateRoomChatRequest(List<int> ParticipantsIds, string? Name = null);