namespace TwitPoster.BLL.DTOs;

public record UserSubscriptionDto(
    AuthorDto User,
    DateTime SubscribedAt
    );