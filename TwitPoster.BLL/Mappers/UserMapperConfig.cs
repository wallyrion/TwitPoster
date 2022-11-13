using Mapster;
using TwitPoster.BLL.DTOs;
using TwitPoster.DAL.Models;

namespace TwitPoster.BLL.Mappers;

public static class UserMapperConfig
{
    public static void RegisterMappings()
    {
        #nullable disable
        TypeAdapterConfig<UserSubscription, UserSubscriptionDto>.NewConfig()
            .Map(dest => dest.User, src => src.Subscription != null ? src.Subscription : src.Subscriber);
    }
}