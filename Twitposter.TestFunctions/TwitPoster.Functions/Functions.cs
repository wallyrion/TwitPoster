using System.Net;
using Azure.Storage.Blobs;
using ImageMagick;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TwitPoster.Functions;

public class Functions(ILogger<Functions> logger)
{
/*, BlobServiceClient blobServiceClient*/
    [Function("Another")]
    public async Task<HttpResponseData> HttpTriggerRun(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query1")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Hello, world!");
        return response;
    }
    
    [Function("One more hhtp trigger")]
    public async Task<HttpResponseData> HttpTriggerRun2(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query1")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Hello, world!");
        return response;
    }

    /*[Function("TriggerForImagesBlob")]
    public async Task BlobTriggerRun(
        [BlobTrigger("twitposter/user/{userId}/images/profile/main/{name}", Connection = "AzureWebJobsStorage")] Stream blob,
        int userId,
        string name)
    {
        try
        {
            logger.LogInformation("Invoking my blob function");
            using var newStream = new MemoryStream();
            await ResizeImage0(blob, newStream);

            var extension = Path.GetExtension(name);
            var container = blobServiceClient.GetBlobContainerClient("twitposter");

            var thumbnailBlobClient = container.GetBlobClient($"user/{userId}/images/profile/thumbnail/image{extension}");

            newStream.Position = 0;
            await thumbnailBlobClient.UploadAsync(newStream, overwrite: true);
            logger.LogInformation("Thumbnail for {UserId} created and uploaded", userId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while processing CompressImageFunction");
            Console.WriteLine(e);

            throw;
        }}*/
    
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
