using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Infrastructure.Auth;
using TwitPoster.Chat.Infrastructure.Persistance;
using TwitPoster.Chat.Infrastructure.SignalR;

namespace TwitPoster.Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddJwtBearerAuthentication(configuration);
        services.AddPersistence(configuration);
        services.AddSignalR(o => o.AddFilter<EnrichUserClaimsFilter>());

        return services;
    }
    
    
}