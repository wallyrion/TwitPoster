using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.RabbitMq;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    public FakeRabbitMqPublisher RabbitMqPublisher { get; private set; } = null!;
    public RabbitMqContainerFixture RabbitMqContainer { get; } = new();
    public MailHogContainerFixture MailHogContainer { get; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(x =>
        {
            var collection = new[]
            {
                KeyValuePair.Create("FeatureManagement:UseRabbitMq", "true"),
                KeyValuePair.Create("RabbitMQ:Host", RabbitMqContainer.HostName),
                KeyValuePair.Create("RabbitMQ:Port", RabbitMqContainer.PublicPort.ToString()),
                KeyValuePair.Create("RabbitMQ:User", RabbitMqContainerFixture.Username),
                KeyValuePair.Create("RabbitMQ:Pass", RabbitMqContainerFixture.Password),
                KeyValuePair.Create("Mail:Host", MailHogContainer.HostName),
                KeyValuePair.Create("Mail:Port", MailHogContainer.SmtpPort.ToString()),
                KeyValuePair.Create("Mail:AuthPassword", "random-password-not-used"),
               
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