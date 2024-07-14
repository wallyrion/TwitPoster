using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TwitPoster.Chat.Infrastructure.SignalR;
using TwitPoster.Chat.IntegrationTests.TestFactories;

namespace TwitPoster.Chat.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public abstract class BaseIntegrationTest(SharedFixtures fixtures) : IAsyncLifetime
{
    private readonly IntegrationTestWebFactory _factory = new(fixtures);
    private AsyncServiceScope _scope;
    protected HttpClient ApiClient { get; private set; } = null!;

    public Task InitializeAsync()
    {
        ApiClient = _factory.CreateDefaultClient();

        _scope = _factory.Services.CreateAsyncScope();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
    
    public string GenerateToken(TestUser user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_factory.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: "TwitPoster.Web",
            audience: "TwitPoster",
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
    
    protected Task AddAuthorization(HttpClient httpClient, TestUser user)
    {
        var token = GenerateToken(user);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return Task.CompletedTask;
    }
    
    protected HubConnection CreateHubConnection(string? token)
    {
        var server = _factory.Server;
        var handler = server.CreateHandler();

        var serverAddressWithoutHttp = server.BaseAddress.Host;
        var uri = new Uri($"ws://{serverAddressWithoutHttp}{ConversationHub.EndpointPath}");

        var queryParams = token != null ? new Dictionary<string, string>
        {
            { "access_token", token }
        }: [];

        var finalUrl = QueryHelpers.AddQueryString(uri.Query, queryParams!);

        var hubConnection = new HubConnectionBuilder()
            .WithUrl(uri + finalUrl, o =>
            {
                o.HttpMessageHandlerFactory = _ => handler;
            })
            .Build();

        return hubConnection;
    }
    
    protected async Task<TaskCompletionSource<ReceivedChatMessage>> SubscribeToMessage(string token)
    {
        var connection = CreateHubConnection(token);
        var receivedTaskCompletionSource = new TaskCompletionSource<ReceivedChatMessage>();
        connection.On<ReceivedChatMessage>(nameof(IConversationClient.ReceivedMessage), (payload) =>
        {
            receivedTaskCompletionSource.SetResult(payload);
        });

        await connection.StartAsync();
        return receivedTaskCompletionSource;
    }
}