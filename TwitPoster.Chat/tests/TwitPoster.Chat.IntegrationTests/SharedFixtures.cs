using Testcontainers.Kafka;
using Testcontainers.MongoDb;

namespace TwitPoster.Chat.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public class SharedFixtures : IAsyncLifetime
{
    public MongoDbContainer MongoContainer { get; } = new MongoDbBuilder().Build();
    public KafkaFixture KafkaContainer { get; } = new();
    
    
    public async Task InitializeAsync()
    {
        //await Task.WhenAll(MongoContainer.StartAsync(), KafkaContainer.StartAsync());
        
        await KafkaContainer.StartAsync();
        
        await MongoContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await MongoContainer.DisposeAsync();
        await KafkaContainer.DisposeAsync();
    }
}