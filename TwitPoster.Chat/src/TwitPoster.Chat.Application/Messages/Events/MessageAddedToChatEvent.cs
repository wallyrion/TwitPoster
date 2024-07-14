namespace TwitPoster.Chat.Application.Messages.Events;

public record MessageAddedToChatEvent(string ChatId, string Text);