using System.Net.Http.Json;
using FluentAssertions;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.IntegrationTests.TestFactories;
using TwitPoster.Chat.Requests;

namespace TwitPoster.Chat.IntegrationTests;

public class ChatsTests(SharedFixtures fixtures) : BaseIntegrationTest(fixtures)
{
    [Theory]
    [InlineData(null)]
    [InlineData("my chat with brothers")]
    public async Task Room_should_be_created(string chatName)
    {
        var chatRoomRequest = new CreateRoomChatRequest([1, 2, 3], chatName);
        var user = TestFactory.CreateUser(1);
        await AddAuthorization(ApiClient, user);
        var createChatResponse = await ApiClient.PostAsJsonAsync("/api/Chats", chatRoomRequest);

        createChatResponse.Should().BeSuccessful();
        createChatResponse.Should().Satisfy<RoomChat>(c =>
        {
            c.Name.Should().Be(chatRoomRequest.Name);
            c.ParticipantsIds.Should().BeEquivalentTo(chatRoomRequest.ParticipantsIds);
        });

        var createdChat = await createChatResponse.Content.ReadFromJsonAsync<RoomChat>();

        var chatRoomByIdResponse = await ApiClient.GetAsync($"api/Chats/{createdChat!.Id}");
        chatRoomByIdResponse.Should().BeSuccessful()
            .And
            .Satisfy<RoomChat>(c =>
            {
                c.Id.Should().Be(createdChat.Id);
                c.Name.Should().Be(chatRoomRequest.Name);
                c.ParticipantsIds.Should().BeEquivalentTo(chatRoomRequest.ParticipantsIds);
            });
    }
}