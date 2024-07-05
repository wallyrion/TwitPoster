using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TwitPoster.BLL.Common.Options;
using TwitPoster.Web.Notifications;
using TwitPoster.Web.Validators;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<RegistrationRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserProfileRequest>, UpdateUserProfileRequestValidator>();
        return services;
    }

    public static IServiceCollection AddSwaggerWithAuthorization(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TwitPoster.Web",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid JWT token (without prefix 'Bearer')",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            c.ExampleFilters();
        });
        
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
        return services;
    }
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, AuthOptions authOptions)
    {
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
                if (path.StartsWithSegments(NotificationHub.EndpointPath))
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
    
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddSingleton(config);

        return services;
    }
}