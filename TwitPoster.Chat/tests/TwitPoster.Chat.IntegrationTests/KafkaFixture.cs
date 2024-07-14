using Testcontainers.Kafka;

namespace TwitPoster.Chat.IntegrationTests;

public class KafkaFixture : IAsyncDisposable
{
    private const string TopicName = "chat-message-received";
    private const string KafkaHost = "test-kafka-host";

    public string BootstrapAddress => KafkaContainer.GetBootstrapAddress();
    
    public KafkaContainer KafkaContainer { get; } = new KafkaBuilder()
        .WithHostname(KafkaHost)
        .Build();
    
    public async Task StartAsync()
    {
        await KafkaContainer.StartAsync();
        //await KafkaContainer.ExecAsync(["kafka-topics", "--create", "--zookeeper", $"{KafkaHost}:{KafkaBuilder.ZookeeperPort}", "--topic", TopicName, "--partitions", "1", "--replication-factor", "1"]);
    }

    public async Task CreateTopic(string name)
    {
        await KafkaContainer.ExecAsync(["kafka-topics", "--create", "--zookeeper", $"{KafkaHost}:{KafkaBuilder.ZookeeperPort}", "--topic", name, "--partitions", "1", "--replication-factor", "1"]);
    }
    
    public async ValueTask DisposeAsync()
    {
        await KafkaContainer.DisposeAsync();
    }
}