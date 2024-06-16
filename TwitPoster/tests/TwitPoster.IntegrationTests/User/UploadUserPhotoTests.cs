using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.DAL;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.User;

public class UploadUserPhotoTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_Upload_Photo_And_Download_By_Url()
    {
        await AddAuthorization();

        var testFilePath = "TestData/Files/photo.jpg";

        // Create multipart content
        var multipartContent = new MultipartFormDataContent();

        await using var fileStream = new FileStream(testFilePath, FileMode.Open);

        // Add file to multipart content
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        multipartContent.Add(fileContent, "file", Path.GetFileName(testFilePath));

        var response = await ApiClient.PostAsync("Users/photo", multipartContent);
        response.Should().Be200Ok();
        var uploadPhotoResponse = await response.Content.ReadFromJsonAsync<UploadPhotoResponse>();
        uploadPhotoResponse.Should().NotBeNull();

        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TwitPosterContext>();
        var user = await dbContext.Users.SingleAsync(u => u.Id == DefaultUserId);
        user.PhotoUrl.Should().NotBeEmpty().And.Be(uploadPhotoResponse!.Url); 

        using var httpClient = new HttpClient();
        var downloadResponse = await httpClient.GetAsync(uploadPhotoResponse.Url);

        downloadResponse.Should().Be200Ok();
        var fileBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        fileBytes.Should().NotBeEmpty();
    }
}
