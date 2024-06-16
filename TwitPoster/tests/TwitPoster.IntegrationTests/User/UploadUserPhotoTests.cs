using FluentAssertions;
using TwitPoster.IntegrationTests.Extensions;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.User;

public class UploadUserPhotoTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_Upload_Photo_And_Download_By_Url()
    {
        await AddAuthorization();

        const string testFilePath = "TestData/Files/photo.jpg";
        var expectedFileBytes = await File.ReadAllBytesAsync(testFilePath);

        // Create multipart content
        var multipartContent = new MultipartFormDataContent();
        using var fileContent = new StreamContent(new MemoryStream(expectedFileBytes));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        multipartContent.Add(fileContent, "file", Path.GetFileName(testFilePath));

        var response = await ApiClient.PostAsync("Users/photo", multipartContent);

        var uploadPhotoResponse = await response
            .Should()
            .Be200Ok()
            .And.Satisfy<UploadPhotoResponse>(r => r.Url.Should().NotBeEmpty())
            .GetJsonResponse<UploadPhotoResponse>();
        
        using var httpClient = new HttpClient();
        var downloadResponse = await httpClient.GetAsync(uploadPhotoResponse!.Url);
        downloadResponse.Should().Be200Ok();

        var actualFileBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        actualFileBytes.Should().NotBeEmpty();
        actualFileBytes.Should().BeEquivalentTo(expectedFileBytes);
    }
}