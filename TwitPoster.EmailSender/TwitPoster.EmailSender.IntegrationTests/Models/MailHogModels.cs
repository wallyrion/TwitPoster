using System.Text.Json.Serialization;

namespace TwitPoster.EmailSender.IntegrationTests.Models;

public class EmailModel
{
    [JsonPropertyName("total")]
    public required int Total { get; set; }

    [JsonPropertyName("count")]
    public required int Count { get; set; }

    [JsonPropertyName("start")]
    public required int Start { get; set; }

    [JsonPropertyName("items")]
    public required List<EmailItem> Items { get; set; }
}

public class EmailItem
{
    [JsonPropertyName("ID")]
    public required string Id { get; set; }

    [JsonPropertyName("From")]
    public required EmailAddress From { get; set; }

    [JsonPropertyName("To")]
    public required List<EmailAddress> To { get; set; }

    [JsonPropertyName("Content")]
    public required EmailContent Content { get; set; }

    [JsonPropertyName("Created")]
    public required DateTime Created { get; set; }

    [JsonPropertyName("MIME")]
    public required object MIME { get; set; } // Assuming MIME might be null or another complex type, adjust as needed

    [JsonPropertyName("Raw")]
    public required RawEmail Raw { get; set; }
}

public class EmailAddress
{
    [JsonPropertyName("Relays")]
    public required object Relays { get; set; } // Assuming Relays might be null or another complex type, adjust as needed

    [JsonPropertyName("Mailbox")]
    public required string Mailbox { get; set; }

    [JsonPropertyName("Domain")]
    public required string Domain { get; set; }

    [JsonPropertyName("Params")]
    public required string Params { get; set; }
}

public class EmailContent
{
    [JsonPropertyName("Headers")]
    public required EmailHeaders Headers { get; set; }

    [JsonPropertyName("Body")]
    public required string Body { get; set; }

    [JsonPropertyName("Size")]
    public required int Size { get; set; }

    [JsonPropertyName("MIME")]
    public required object MIME { get; set; } // Assuming MIME might be null or another complex type, adjust as needed
}

public class EmailHeaders
{
    [JsonPropertyName("Content-Type")]
    public required List<string> ContentType { get; set; }

    [JsonPropertyName("Date")]
    public required List<string> Date { get; set; }

    [JsonPropertyName("From")]
    public required List<string> From { get; set; }

    [JsonPropertyName("MIME-Version")]
    public required List<string> MimeVersion { get; set; }

    [JsonPropertyName("Message-Id")]
    public required List<string> MessageId { get; set; }

    [JsonPropertyName("Received")]
    public required List<string> Received { get; set; }

    [JsonPropertyName("Return-Path")]
    public required List<string> ReturnPath { get; set; }

    [JsonPropertyName("Subject")]
    public required List<string> Subject { get; set; }

    [JsonPropertyName("To")]
    public required List<string> To { get; set; }
}

public class RawEmail
{
    [JsonPropertyName("From")]
    public required string From { get; set; }

    [JsonPropertyName("To")]
    public required List<string> To { get; set; }

    [JsonPropertyName("Data")]
    public required string Data { get; set; }

    [JsonPropertyName("Helo")]
    public required string Helo { get; set; }
}
