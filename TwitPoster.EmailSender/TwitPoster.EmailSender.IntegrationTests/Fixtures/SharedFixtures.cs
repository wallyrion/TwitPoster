namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

[Collection(nameof(IntegrationTestsCollection))]
public class SharedFixtures : IAsyncLifetime
{
    public FakeRabbitMqPublisher RabbitMqPublisher { get; private set; } = null!;
    public RabbitMqContainerFixture RabbitMqContainer { get; } = new();
    public MailHogContainerFixture MailHogContainer { get; } = new();
    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.StartAsync(), MailHogContainer.InitializeAsync());
        
        RabbitMqPublisher = new FakeRabbitMqPublisher(RabbitMqContainer);
    }
    
    public async Task DisposeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.DisposeAsync().AsTask(), MailHogContainer.DisposeAsync());
    }
}