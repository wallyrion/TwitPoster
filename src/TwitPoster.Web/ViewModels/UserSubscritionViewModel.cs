namespace TwitPoster.Web.ViewModels;

public record UserSubscriptionViewModel(
    AuthorViewModel User,
    DateTime SubscribedAt
);