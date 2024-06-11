using System.Reflection;
using MassTransit;
using TwitPoster.EmailSender.Consumer;
using TwitPoster.EmailSender.Options;
using TwitPoster.Shared.Contracts;

namespace TwitPoster.EmailSender.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,  ConfigurationManager configuration)
    {
        var featureFlags = configuration.BindOption<FeatureFlagsOptions>(services, false);

        if (featureFlags.UseRabbitMq)
        {
            var rabbitMqConfig = configuration.BindOption<RabbitMqOptions>(services, false);
            services
                .Configure<RabbitMqTransportOptions>(options =>
                {
                    options.Host = rabbitMqConfig.Host;
                });
        }
        
        services.AddMassTransit(x =>
        {
            var entryAssembly = Assembly.GetExecutingAssembly();
            x.AddConsumers(entryAssembly);

            if (featureFlags.UseRabbitMq)
            {
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
            }
            else
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("ServiceBus"));

                    cfg.UseServiceBusMessageScheduler();
                    cfg.SubscriptionEndpoint<EmailCommand>($"EmailSenderSubscription",
                        configurator => { configurator.ConfigureConsumer<EmailCommandConsumer>(context); });
                });
            }
        });

        return services;
    }
}