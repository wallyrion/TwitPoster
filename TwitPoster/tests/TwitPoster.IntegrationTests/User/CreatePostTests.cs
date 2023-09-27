using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.DAL;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web.ViewModels;
using TwitPoster.Web.ViewModels.Post;

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
        response.Should().Be204NoContent();

        var newContext = Scope.ServiceProvider.CreateAsyncScope();
        var dbContext = newContext.ServiceProvider.GetRequiredService<TwitPosterContext>();
        var user = await dbContext.Users.SingleAsync(u => u.Id == DefaultUserId);
        var photoUrl = user.PhotoUrl;

        photoUrl.Should().NotBeEmpty();
        
        using var httpClient = new HttpClient();
        var downloadResponse = await httpClient.GetAsync(photoUrl);

        downloadResponse.Should().Be200Ok();
        var fileBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        fileBytes.Should().NotBeEmpty();
    }

    
   
}
