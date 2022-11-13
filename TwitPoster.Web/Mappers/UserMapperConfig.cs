using Mapster;
using TwitPoster.BLL.DTOs;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Mappers;

public static class UserMapperConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<AuthorDto, AuthorViewModel>
            .NewConfig()
            .Map(dest => dest.Fullname, src => $"{src.FirstName} {src.LastName}");
    }
}
