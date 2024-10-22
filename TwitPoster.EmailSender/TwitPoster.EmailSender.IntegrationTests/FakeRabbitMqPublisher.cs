using MassTransit;
using Microsoft.Extensions.DependencyInjection;

using TwitPoster.EmailSender.IntegrationTests.Fixtures;

namespace TwitPoster.EmailSender.IntegrationTests;

public class FakeRabbitMqPublisher
{
    public ServiceProvider Provider { get; private set; }
    public FakeRabbitMqPublisher(RabbitMqContainerFixture rabbitMqContainer)
    {
        var services = new ServiceCollection();

        services.Configure<RabbitMqTransportOptions>(options =>
        {
            options.Host = rabbitMqContainer.HostName;
            options.Port = rabbitMqContainer.PublicPort;
            options.User = RabbitMqContainerFixture.Username;
            options.Pass = RabbitMqContainerFixture.Password;
        });

       // services.AddLogging(builder => builder.AddSerilog().AddConsole());
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq();
        });
        
        Provider = services.BuildServiceProvider();
    }
}