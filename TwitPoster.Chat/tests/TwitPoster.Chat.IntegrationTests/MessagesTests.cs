using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using TwitPoster.Chat.Domain.ChatAggregateRoot;
using TwitPoster.Chat.Infrastructure.SignalR;
using TwitPoster.Chat.IntegrationTests.TestFactories;
using TwitPoster.Chat.Requests;

namespace TwitPoster.Chat.IntegrationTests;

public class MessagesTests(SharedFixtures fixtures) : BaseIntegrationTest(fixtures, isNewTopicNeeded: true)
{
    [Fact]
    public async Task Connect_to_hub_and_send_message_should_send_message_to_participants_and_save_to_db()
    {
        var chatRoomRequest = new CreateRoomChatRequest([1, 2, 3], "my chat with brothers");
        var user1 = TestFactory.CreateUser(1);
        var token1 = GenerateToken(user1);
        await AddAuthorization(ApiClient, user1);
        var createChatResponse = await ApiClient.PostAsJsonAsync("/api/Chats", chatRoomRequest);
        var createdChat = await createChatResponse.Content.ReadFromJsonAsync<RoomChat>();

        var user2 = TestFactory.CreateUser(2);
        var token2 = GenerateToken(user2);
        var connection1 = CreateHubConnection(token1);
        await connection1.StartAsync();
        var subscriber2 = await SubscribeToMessage(token2);

        var testMessage = new SentChatMessage(createdChat!.Id, Guid.NewGuid().ToString());
        await connection1.SendAsync(nameof(ConversationHub.Hello), testMessage);
        var result = await subscriber2.WaitForResult(TimeSpan.FromSeconds(10));
        
        result.Should().BeEquivalentTo(new
        {
            ChatId = createdChat.Id,
            Text = testMessage.Text
        });
        
        var messages = await ApiClient.GetAsync($"/api/Chats/{createdChat.Id}/messages");
        var actualMessage = ((await messages.Content.ReadFromJsonAsync<List<TestMessageDto>>())!).Last();
        actualMessage.Should().BeEquivalentTo(new
        {
            Text = testMessage.Text,
            AuthorId = user1.Id
        });

        actualMessage.Created.Should().BeAfter(DateTime.UtcNow.AddDays(-1)).And.BeBefore(DateTime.UtcNow);
    }
}