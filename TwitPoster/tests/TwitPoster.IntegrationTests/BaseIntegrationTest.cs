using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    protected IServiceScope Scope = null!;
    protected TwitPosterContext DbContext = null!;
    protected HttpClient HttpClient = null!;
    protected int DefaultUserId = 1;
    
    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        
        var webFactory = new WebApplicationFactory<IApiTestMarker>()
            .WithWebHostBuilder(b =>
            {
                b.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TwitPosterContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                
                    services.AddDbContext<TwitPosterContext>(options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, _msSqlContainer.GetConnectionString()));
                });
            });
        HttpClient = webFactory.CreateClient();
        Scope = webFactory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<TwitPosterContext>();
    }

    protected async Task AddAuthorization()
    {
        var user = await DbContext.Users.Include(x => x.UserAccount).FirstAsync(user => user.Id == DefaultUserId);
        var jwtGenerator = Scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
        var token = jwtGenerator.GenerateToken(user);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}