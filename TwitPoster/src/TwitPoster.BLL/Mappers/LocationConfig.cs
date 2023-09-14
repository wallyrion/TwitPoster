using Mapster;
using TwitPoster.BLL.DTOs.Location;
using TwitPoster.BLL.External.Location;

namespace TwitPoster.BLL.Mappers;

public class LocationConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CountryResponseItem, Country>()
            .Map(dest => dest.Code, src => src.Iso2)
            .Map(dest => dest.UnicodeFlag, src => src.UnicodeFlag)
            .Map(dest => dest.Name, src => src.Name);
        
        config.NewConfig<CountriesResponse, IReadOnlyList<Country>>()
            .ConstructUsing(x => x.Data.Adapt<IReadOnlyList<Country>>())
            ;
    }
}