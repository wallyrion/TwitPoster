namespace TwitPoster.DAL.Models;

public class UserSubscription
{
    public User Subscriber { get; set; } = null!;
    public int SubscriberId { get; set; }
    public User Subscription { get; set; } = null!;
    public int SubscriptionId { get; set; }
    public DateTime SubscribedAt { get; set; }
}