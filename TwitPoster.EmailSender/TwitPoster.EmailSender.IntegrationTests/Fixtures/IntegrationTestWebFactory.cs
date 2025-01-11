using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public class IntegrationTestWebFactory(SharedFixtures fixtures) : WebApplicationFactory<IApiTestMarker>
{
    public FakeRabbitMqPublisher RabbitMqPublisher => fixtures.RabbitMqPublisher;
    public RabbitMqContainerFixture RabbitMqContainer => fixtures.RabbitMqContainer;
    public MailHogContainerFixture MailHogContainer => fixtures.MailHogContainer;

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
}