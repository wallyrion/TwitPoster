using Microsoft.Extensions.DependencyInjection;
using TwitPoster.EmailSender.IntegrationTests.Fixtures;

namespace TwitPoster.EmailSender.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public abstract class BaseIntegrationTest(SharedFixtures fixtures) : IAsyncLifetime
{
    protected AsyncServiceScope Scope { get; private set; }
    protected readonly IntegrationTestWebFactory WebApiFactory  = new(fixtures);
    
    public Task InitializeAsync()
    {
        Scope = WebApiFactory.Services.CreateAsyncScope();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
        await WebApiFactory.MailHogContainer.ClearReceivedMessages();
    }
}
    