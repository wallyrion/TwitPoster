using Azure.Storage.Blobs;
using ImageMagick;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TwitPoster.Functions;

public class Functions
{
    private readonly ILogger _logger;
    private readonly BlobServiceClient _blobServiceClient;
    public Functions(ILoggerFactory loggerFactory, BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
        _logger = loggerFactory.CreateLogger<Functions>();
    }

    [Function("CompressImageFunction")]
    public async Task Run([BlobTrigger("twitposter-local/user/{userId}/images/profile/main/{name}", Connection = "AzureWebJobsStorage")] Stream blob, int userId, string name)
    {
        // Upload the thumbnail to the "profile-thumbnails" container

        using var newStream = new MemoryStream();
        await ResizeImage0(blob, newStream);

        var extension = Path.GetExtension(name);

        var thumbnailBlobClient = _blobServiceClient.GetBlobContainerClient("twitposter-local").GetBlobClient($"user/{userId}/images/profile/thumbnail/image{extension}");

        newStream.Position = 0;
        await thumbnailBlobClient.UploadAsync(newStream, overwrite: true);
        _logger.LogInformation("Thumbnail for {UserId} created and uploaded", userId);
    }

    private async Task ResizeImage0(Stream stream, Stream blob)
    {
        const int size = 200;
        const int quality = 20;

        using var image = new MagickImage(stream);

        image.Resize(size, size);
        image.Quality = quality;

        await image.WriteAsync(blob);
    }

}