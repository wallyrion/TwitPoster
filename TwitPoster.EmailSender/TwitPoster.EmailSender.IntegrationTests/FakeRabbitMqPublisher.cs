using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Testcontainers.RabbitMq;

namespace TwitPoster.EmailSender.IntegrationTests;

public class FakeRabbitMqPublisher
{
    public ServiceProvider Provider { get; private set; }
    public FakeRabbitMqPublisher(RabbitMqContainer rabbitMqContainer)
    {
        var services = new ServiceCollection();

        services.Configure<RabbitMqTransportOptions>(options =>
        {
            options.Host = rabbitMqContainer.Hostname;
            options.Port = rabbitMqContainer.GetMappedPublicPort(5672);
            options.User = "guest";
            options.Pass = "guest";
        });

        services.AddLogging(builder => builder.AddSerilog().AddConsole());
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq();
        });
        
        Provider = services.BuildServiceProvider();
    }
}