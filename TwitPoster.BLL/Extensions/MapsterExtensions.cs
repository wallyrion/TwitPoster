using Mapster;

namespace TwitPoster.BLL.Extensions;

public static class TypeAdapterHelper
{
    public static TypeAdapterSetter<TSource,TDestination> Override<TSource, TDestination>(out TypeAdapterConfig config)
    {
        var mapConfig = TypeAdapterConfig.GlobalSettings.Clone();
        config = mapConfig;
        
        return mapConfig.ForType<TSource, TDestination>();
    } 
}