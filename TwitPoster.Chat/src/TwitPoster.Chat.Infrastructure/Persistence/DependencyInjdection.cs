using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.Chat.Application.Common.Interfaces;

namespace TwitPoster.Chat.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.BindOption<DatabaseSettings>(services);
        services.AddSingleton<IMessagesRepository, MessagesRepository>();
        services.AddSingleton<IChatsRepository, ChatsRepository>();

        return services;
    }
    
    
}