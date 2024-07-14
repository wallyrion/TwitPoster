using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TwitPoster.Chat.Infrastructure.SignalR;

namespace TwitPoster.Chat.Infrastructure.Auth;

internal static class DependencyInjection
{
    internal static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.BindOption<AuthOptions>(services);
        
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authOptions.Secret))
                };
                
                options.Events = SubscribeToAuthEvents();
            });
        
        return services;
    }
    
    private static JwtBearerEvents SubscribeToAuthEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (!string.IsNullOrEmpty(ctx.Token))
                {
                    return Task.CompletedTask;
                }

                var path = ctx.HttpContext.Request.Path;

                // Look for token in query string for SignalR hubs
                if (path.StartsWithSegments(ConversationHub.EndpointPath))
                {
                    var token = ctx.Request.Query["access_token"];

                    if (!string.IsNullOrEmpty(token))
                    {
                        ctx.Token = token;
                    }
                }

                return Task.CompletedTask;
            }
        };
    }

}