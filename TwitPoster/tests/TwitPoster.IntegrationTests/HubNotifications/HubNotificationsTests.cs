using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Notifications;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web.Notifications;

namespace TwitPoster.IntegrationTests.HubNotifications;

public class HubNotificationsTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    private readonly IntegrationTestWebFactory _factory = factory;
    
    [Fact]
    public async Task Throw_unauthorized_exception_when_token_not_provided()
    {
        var action = () => SubscribeToConnection(null!);
        
        await action.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Response status code does not indicate success: 401 (Unauthorized).");
    }
    
    [Fact]
    public async Task Create_Post_should_return_Unauthorized_for_anonymous()
    {
        var postAuthor = Data.BaseFixture.Create<DAL.Models.User>();
        DbContext.Users.Add(postAuthor);
        await DbContext.SaveChangesAsync();

        var expectedPost = Data.BaseFixture.Create<DAL.Models.Post>();
        expectedPost.AuthorId = postAuthor.Id;
        DbContext.Posts.Add(expectedPost);
        await DbContext.SaveChangesAsync();

        var token = await GenerateToken(postAuthor.Id);
        var tokenSource = await SubscribeToConnection(token);
        
        await AddAuthorization();
        var response = await ApiClient.PutAsync($"Posts/{expectedPost.Id}/like", null);
        response.Should().Be200Ok();

        var resultTask = await Task.WhenAny(tokenSource.Task, Task.Delay(1000));

        var likedByUser = await DbContext.Users
            .FirstAsync(u => u.Id == DefaultUserId);
        
        resultTask.Should().Be(tokenSource.Task, "Notification was not received");
        (await tokenSource.Task).Item1.Should().Be(NotificationType.LikedPost);
        (await tokenSource.Task).Item2.Should().BeEquivalentTo(new
        {
            ByUserName = $"{likedByUser.FirstName} {likedByUser.LastName}",
        });
    }


    private async Task<string> GenerateToken(int userId)
    {
        var user = await DbContext.Users.Include(x => x.UserAccount).FirstAsync(user => user.Id == userId);
        var jwtGenerator = Scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
        var token = jwtGenerator.GenerateToken(user);

        return token;
    }
    
    
    private async Task<TaskCompletionSource<(NotificationType, NotificationPayload)>> SubscribeToConnection(string token)
    {
        var connection = CreateHubConnection(token);
        var receivedTaskCompletionSource = new TaskCompletionSource<(NotificationType, NotificationPayload)>();
        connection.On<NotificationType, NotificationPayload>(nameof(INotificationClient.SendNotification), (type, payload) =>
        {
            receivedTaskCompletionSource.SetResult((type, payload));
        });

        await connection.StartAsync();
        return receivedTaskCompletionSource;
    }
    
    private HubConnection CreateHubConnection(string? token)
    {
        var server = _factory.Server;
        var handler = server.CreateHandler();

        var serverAddressWithoutHttp = server.BaseAddress.Host;
        var uri = new Uri($"ws://{serverAddressWithoutHttp}{NotificationHub.EndpointPath}");

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
}
