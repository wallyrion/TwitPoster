using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Application.Messages.Events;
using TwitPoster.Chat.Infrastructure.Auth;
using TwitPoster.Chat.Infrastructure.Common;
using TwitPoster.Chat.Infrastructure.Kafka;
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
                var kafkaOptions = configuration.BindOption<KafkaOptions>(services, false);
                
                rider.AddConsumer<MessageAddedToChatConsumer>();
                rider.AddProducer<string, MessageAddedToChatEvent>(kafkaOptions.Topic);
                
                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaOptions.Host);
                    k.TopicEndpoint<MessageAddedToChatEvent>(kafkaOptions.Topic, "chat-message-gr", e =>
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