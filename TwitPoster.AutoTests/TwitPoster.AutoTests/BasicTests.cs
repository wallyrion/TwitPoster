using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TwitPoster.AutoTests.Models;

namespace TwitPoster.AutoTests;

public class BasicTests
{
    private readonly TestData _data = new();
    
    [Fact]
    public async Task GetPosts_Retunrs_Ok()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Configuration.TwitposterApiBaseUrl);

        var response = await httpClient.GetAsync("posts");

        response.Should().Be200Ok();
    }
    
    [Fact]
    public async Task Get_Logins_And_Create_Post()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Configuration.TwitposterApiBaseUrl);

        var loginResponse = await httpClient.PostAsJsonAsync("auth/login", new { email = Configuration.TestUserEmail, password = Configuration.TestUserPassword });
        loginResponse.Should().Be200Ok();
        
        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.AccessToken);
        
        var createPostResponse = await httpClient.PostAsJsonAsync("posts", new { Body = _data.PostBody });
        createPostResponse.Should().Be200Ok();

        var postContent = await createPostResponse.Content.ReadFromJsonAsync<PostViewModel>();
        postContent.Should().NotBeNull();
        postContent!.Body.Should().Be(_data.PostBody);
        
        var createCommentResponse = await httpClient.PostAsJsonAsync($"posts/{postContent.Id}/comments", new { Text = _data.CommentBody });
        createCommentResponse.Should().Be200Ok();
        var commentContent = await createCommentResponse.Content.ReadFromJsonAsync<PostCommentViewModel>();
        commentContent!.Text.Should().Be(_data.CommentBody);
    }
}