using System.Net.Http.Json;
using FluentAssertions;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.IntegrationTests.TestFactories;

namespace TwitPoster.Chat.IntegrationTests;

public class MessagesTests(SharedFixtures fixtures) : BaseIntegrationTest(fixtures)
{
    [Fact]
    public async Task Connect_to_hub_and_send_message_should_send_message_to_participants_and_save_to_db()
    {
        var chatRoomRequest = new CreateRoomChatRequest([1, 2, 3], "my chat with brothers");
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