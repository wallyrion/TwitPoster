using Mapster;
using TwitPoster.BLL.DTOs;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Mappers;

public class UserMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuthorDto, AuthorViewModel>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
    }
}
