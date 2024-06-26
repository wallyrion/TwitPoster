using Azure.Storage.Blobs;
using ImageMagick;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TwitPoster.Functions;

public class Functions(ILoggerFactory loggerFactory, BlobServiceClient blobServiceClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<Functions>();

    [Function("CompressImageFunction")]
    public async Task Run([BlobTrigger("twitposter/user/{userId}/images/profile/main/{name}", Connection = "AzureWebJobsStorage")] Stream blob, int userId,
        string name)
    {
        try
        {
            using var newStream = new MemoryStream();
            await ResizeImage0(blob, newStream);

            var extension = Path.GetExtension(name);

            var container = blobServiceClient.GetBlobContainerClient("twitposter");
            
            var thumbnailBlobClient = container.GetBlobClient($"user/{userId}/images/profile/thumbnail/image{extension}");

            newStream.Position = 0;
            await thumbnailBlobClient.UploadAsync(newStream, overwrite: true);
            _logger.LogInformation("Thumbnail for {UserId} created and uploaded", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing CompressImageFunction");
            Console.WriteLine(e);

            throw;
        }
    }

    private static async Task ResizeImage0(Stream stream, Stream blob)
    {
        const int size = 200;
        const int quality = 20;
        using var image = new MagickImage(stream);

        image.Resize(size, size);
        image.Quality = quality;

        await image.WriteAsync(blob);
    }
}