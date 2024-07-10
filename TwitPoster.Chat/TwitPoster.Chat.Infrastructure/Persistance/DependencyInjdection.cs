﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.Chat.Application;

namespace TwitPoster.Chat.Infrastructure.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.BindOption<DatabaseSettings>(services);
        services.Configure<DatabaseSettings>(configuration.GetSection("BookStoreDatabase"));
        services.AddScoped<IMessagesRepository, MessagesRepository>();

        return services;
    }
    
    
}