using System.Net.Http.Json;
using FluentAssertions;
using TwitPoster.Chat.Domain.ChatAggregateRoot;
using TwitPoster.Chat.IntegrationTests.TestFactories;
using TwitPoster.Chat.Requests;

namespace TwitPoster.Chat.IntegrationTests;

public class ChatsTests(SharedFixtures fixtures) : BaseIntegrationTest(fixtures)
{
    [Theory]
    [InlineData(null)]
    [InlineData("my chat with brothers")]
    public async Task Chat_should_be_created(string? chatName)
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
    
    [Fact]
    public async Task Should_only_visible_chats_when_user_is_participant()
    {
        var chatRoomRequest = new CreateRoomChatRequest([1, 3], "something");
        var user = TestFactory.CreateUser(1);
        await AddAuthorization(ApiClient, user);
        var createChatResponse = await ApiClient.PostAsJsonAsync("/api/Chats", chatRoomRequest);
        var createdChat = await createChatResponse.Content.ReadFromJsonAsync<RoomChat>();

        var chatsForUser1Response = await ApiClient.GetAsync($"api/Chats");
        chatsForUser1Response.Should().BeSuccessful()
            .And
            .Satisfy<List<RoomChat>>(c =>
            {
                c.Should().Contain(x => x.Id == createdChat!.Id);
            });
        
        var user2 = TestFactory.CreateUser(2);
        await AddAuthorization(ApiClient, user2);
        
        var chatRoomByIdResponse = await ApiClient.GetAsync($"api/Chats");
        chatRoomByIdResponse.Should().BeSuccessful()
            .And
            .Satisfy<List<RoomChat>>(c =>
            {
                c.Should().NotContain(x => x.Id == createdChat!.Id);
            });
    }
    
}