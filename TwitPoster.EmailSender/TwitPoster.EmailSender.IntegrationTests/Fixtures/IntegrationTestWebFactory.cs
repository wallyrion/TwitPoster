using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.RabbitMq;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    public FakeRabbitMqPublisher RabbitMqPublisher { get; private set; } = null!;
    public RabbitMqContainer RabbitMqContainer { get; } = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithPortBinding(15672, true)
        .WithUsername("guest").WithPassword("guest").Build();
    public MailHogContainerFixture MailHogContainer { get; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(x =>
        {
            Console.WriteLine("RabbitMqContainer.Hostname: " + RabbitMqContainer.Hostname);
            Console.WriteLine("RabbitMqContainer.GetMappedPublicPort(5672): " + RabbitMqContainer.GetMappedPublicPort(5672));
            Console.WriteLine("MailHogContainer.HostName: " + MailHogContainer.HostName);
            Console.WriteLine("MailHogContainer.SmtpPort: " + MailHogContainer.SmtpPort);
            var collection = new[]
            {
                KeyValuePair.Create("FeatureManagement:UseRabbitMq", "true"),
                KeyValuePair.Create("RabbitMQ:Host", RabbitMqContainer.Hostname),
                KeyValuePair.Create("RabbitMQ:Port", RabbitMqContainer.GetMappedPublicPort(5672).ToString()),
                KeyValuePair.Create("RabbitMQ:User", "guest"),
                KeyValuePair.Create("RabbitMQ:Pass", "guest"),
                KeyValuePair.Create("Mail:Host", MailHogContainer.HostName),
                KeyValuePair.Create("Mail:Port", MailHogContainer.SmtpPort.ToString()),
               
            };
            x.AddInMemoryCollection(collection!);
        });
        
        return base.CreateHost(builder);
    }
    
    
    public async Task InitializeAsync()
    {
        await RabbitMqContainer.StartAsync();
        await MailHogContainer.InitializeAsync();
        
        RabbitMqPublisher = new FakeRabbitMqPublisher(RabbitMqContainer);
    }

    public new async Task DisposeAsync()
    {
        await RabbitMqContainer.DisposeAsync();
        await MailHogContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}