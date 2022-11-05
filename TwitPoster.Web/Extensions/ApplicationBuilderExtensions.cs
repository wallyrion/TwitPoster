namespace TwitPoster.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder InDevelopment(this WebApplication builder, Func<IApplicationBuilder, IApplicationBuilder> registerMiddlewareFunc)
    {
        return builder.Environment.IsDevelopment() 
            ? registerMiddlewareFunc(builder)
            : builder;
    }
}