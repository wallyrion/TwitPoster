using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TwitPoster.Chat.IntegrationTests;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiMarker>
{
    public readonly string Secret = "supersecretkey" + Guid.NewGuid();
    private readonly SharedFixtures _fixtures;
    public string? KafkaTopicName { get; set; }
    
    public IntegrationTestWebFactory(SharedFixtures fixtures)
    {
        _fixtures = fixtures;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(x =>
        {
            var collection = new[]
            {
                KeyValuePair.Create("TwitPosterChatDb:ConnectionString", _fixtures.MongoContainer.GetConnectionString()),
                KeyValuePair.Create("Auth:Secret", Secret),
                KeyValuePair.Create("Kafka:Host", _fixtures.KafkaContainer.BootstrapAddress),
                KeyValuePair.Create("Kafka:Topic", KafkaTopicName ?? "default-topic")
            };
            x.AddInMemoryCollection(collection!);
        });
        
        return base.CreateHost(builder);
    }
}