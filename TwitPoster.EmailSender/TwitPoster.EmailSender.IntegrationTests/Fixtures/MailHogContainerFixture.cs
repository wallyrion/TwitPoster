using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using TwitPoster.EmailSender.IntegrationTests.Models;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public class MailHogContainerFixture : IAsyncLifetime
{
    private readonly IContainer _container;

    private const int InternalSmtpPort = 1025;
    public const int InternalHttpPort = 8025;

    public MailHogContainerFixture()
    {
        var container = new ContainerBuilder()
            .WithImage("mailhog/mailhog")
            .WithPortBinding(InternalSmtpPort, true)
            .WithPortBinding(InternalHttpPort, true);

        _container = container.Build();
    }

    public string HostName => _container.Hostname;
    public int SmtpPort => _container.GetMappedPublicPort(InternalSmtpPort);
    public int HttpPort => _container.GetMappedPublicPort(InternalHttpPort);

    public async Task ClearReceivedMessages()
    {
        await ClearAllMessages();
    }

    public async Task<EmailModel> GetReceivedMessages()
    {
        return await GetEmailsAsync();
    }

    public Task<EmailItem> WaitForNewMessage(TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(10);

        var cts = new CancellationTokenSource(timeout.Value);

        var task = Task.Run(async () =>
        {
            var emails = await GetReceivedMessages();
            HashSet<string> ids = [..emails.Items.Select(x => x.Id)];

            while (true)
            {
                await Task.Delay(50, cts.Token);
                emails = await GetReceivedMessages();

                var newEmail = emails.Items.FirstOrDefault(x => !ids.Contains(x.Id));

                if (newEmail != null)
                {
                    return newEmail;
                }
            }
        }, cts.Token);

        return task;
    }

    private async Task<EmailModel> GetEmailsAsync()
    {
        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri($"http://{HostName}:{HttpPort}/api/");

        HttpResponseMessage response = await httpClient.GetAsync("v2/messages");

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<EmailModel>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return jsonResponse!;
    }

    private async Task ClearAllMessages()
    {
        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri($"http://{HostName}:{HttpPort}/api/");

        HttpResponseMessage response = await httpClient.DeleteAsync("v1/messages");

        response.EnsureSuccessStatusCode();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}