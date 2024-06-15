using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ImageMagick;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TwitPoster.Functions
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobServiceClient;
        public Function1(ILoggerFactory loggerFactory, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        /*
        [Function("Function1")]
        public void Run([BlobTrigger("twitposter-local", Connection = "AzureWebJobsStorage")] Stream inputBlob)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name:");
        }
        */


        [Function("Function")]
        public async Task Run([BlobTrigger("twitposter-local/user/{userId}/images/profile/main/{name}", Connection = "AzureWebJobsStorage")] Stream blob, int userId, string name)
        {
            // Upload the thumbnail to the "profile-thumbnails" container

            using var newStream = new MemoryStream();
            await ResizeImage0(blob, newStream);

            var extension = Path.GetExtension(name);

            var thumbnailBlobClient = _blobServiceClient.GetBlobContainerClient("twitposter-local").GetBlobClient($"user/{userId}/images/profile/thumbnail/image{extension}");

            newStream.Position = 0;
            thumbnailBlobClient.Upload(newStream, overwrite: true);
            _logger.LogInformation($"Thumbnail for {userId} created and uploaded");

           

            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name:");
        }


        private async Task ResizeImage0(Stream stream, Stream blob)
        {


            const int size = 200;
            const int quality = 20;

            /*var inputPath = "input.jpg";
            var outputPath = "output.jpg";*/


            using (var image = new MagickImage(stream))
            {
                image.Resize(size, size);
                //image.Strip();
                image.Quality = quality;


                image.Write(blob);
            }

        }

    }
}

