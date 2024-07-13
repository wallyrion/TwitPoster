using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TwitPoster.Chat.IntegrationTests.TestFactories;

namespace TwitPoster.Chat.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly SharedFixtures _fixtures;
    protected readonly IntegrationTestWebFactory Factory;
    private AsyncServiceScope _scope;
    protected HttpClient ApiClient { get; private set; } = null!;
    
    protected BaseIntegrationTest(SharedFixtures fixtures)
    {
        _fixtures = fixtures;
        Factory = new IntegrationTestWebFactory(_fixtures);
    }

    public async Task InitializeAsync()
    {
        ApiClient = Factory.CreateDefaultClient();

        _scope = Factory.Services.CreateAsyncScope();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
    
    public string GenerateToken(TestUser user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Factory.Secret)),
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
    
    protected async Task AddAuthorization(HttpClient httpClient, TestUser user)
    {
        var token = GenerateToken(user);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}