using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace TwitPoster.EmailSender.IntegrationTests;

public class MailHogContainerFixture : IAsyncLifetime
{
    private readonly IContainer _container;
    
    const int InternalSmtpPort = 1025;
    const int InternalHttpPort = 8025;
    
    public MailHogContainerFixture()
    {
        var container = new ContainerBuilder()
                .WithImage("mailhog/mailhog")
                .WithPortBinding(InternalSmtpPort, true)
                .WithPortBinding(InternalHttpPort, true)
            ;
        
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
    
    /*public async TaskCompletionSource<List<EmailModel>> WaitForNewMessage()
    {
        var res = await GetEmailsAsync();
    }*/
    
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


public class EmailModel
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("items")]
    public List<EmailItem> Items { get; set; }
}

public class EmailItem
{
    [JsonPropertyName("ID")]
    public string ID { get; set; }

    [JsonPropertyName("From")]
    public EmailAddress From { get; set; }

    [JsonPropertyName("To")]
    public List<EmailAddress> To { get; set; }

    [JsonPropertyName("Content")]
    public EmailContent Content { get; set; }

    [JsonPropertyName("Created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("MIME")]
    public object MIME { get; set; } // Assuming MIME might be null or another complex type, adjust as needed

    [JsonPropertyName("Raw")]
    public RawEmail Raw { get; set; }
}

public class EmailAddress
{
    [JsonPropertyName("Relays")]
    public object Relays { get; set; } // Assuming Relays might be null or another complex type, adjust as needed

    [JsonPropertyName("Mailbox")]
    public string Mailbox { get; set; }

    [JsonPropertyName("Domain")]
    public string Domain { get; set; }

    [JsonPropertyName("Params")]
    public string Params { get; set; }
}

public class EmailContent
{
    [JsonPropertyName("Headers")]
    public EmailHeaders Headers { get; set; }

    [JsonPropertyName("Body")]
    public string Body { get; set; }

    [JsonPropertyName("Size")]
    public int Size { get; set; }

    [JsonPropertyName("MIME")]
    public object MIME { get; set; } // Assuming MIME might be null or another complex type, adjust as needed
}

public class EmailHeaders
{
    [JsonPropertyName("Content-Type")]
    public List<string> ContentType { get; set; }

    [JsonPropertyName("Date")]
    public List<string> Date { get; set; }

    [JsonPropertyName("From")]
    public List<string> From { get; set; }

    [JsonPropertyName("MIME-Version")]
    public List<string> MimeVersion { get; set; }

    [JsonPropertyName("Message-Id")]
    public List<string> MessageId { get; set; }

    [JsonPropertyName("Received")]
    public List<string> Received { get; set; }

    [JsonPropertyName("Return-Path")]
    public List<string> ReturnPath { get; set; }

    [JsonPropertyName("Subject")]
    public List<string> Subject { get; set; }

    [JsonPropertyName("To")]
    public List<string> To { get; set; }
}

public class RawEmail
{
    [JsonPropertyName("From")]
    public string From { get; set; }

    [JsonPropertyName("To")]
    public List<string> To { get; set; }

    [JsonPropertyName("Data")]
    public string Data { get; set; }

    [JsonPropertyName("Helo")]
    public string Helo { get; set; }
}
