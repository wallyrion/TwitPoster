using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Application.Messages.Events;
using TwitPoster.Chat.Infrastructure.Auth;
using TwitPoster.Chat.Infrastructure.Consumers;
using TwitPoster.Chat.Infrastructure.Persistence;
using TwitPoster.Chat.Infrastructure.SignalR;

namespace TwitPoster.Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IRealtimeNotifier, RealtimeNotifier>();

        services.AddJwtBearerAuthentication(configuration);
        services.AddPersistence(configuration);
        services.AddSignalR(o => o.AddFilter<EnrichUserClaimsFilter>());

        services.AddMassTransit(x =>
        {
            x.UsingInMemory();
            
            x.AddRider(rider =>
            {
                rider.AddConsumer<MessageAddedToChatConsumer>();
                rider.AddProducer<string, MessageAddedToChatEvent>("chat-message-received");
                
                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");
                    k.TopicEndpoint<MessageAddedToChatEvent>("chat-message-received", "chat-message-gr", e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;

                        e.ConcurrentMessageLimit = 10;
                        e.ConfigureConsumer<MessageAddedToChatConsumer>(context);
                    });
                });
            });
        });
        
        return services;
    }
    
    //
}