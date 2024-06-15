using Azure.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitPoster.Functions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var section = context.Configuration.GetRequiredSection("Storage");
        var storageOptions = section.Get<StorageOptions>();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(new Uri(storageOptions!.Uri), new StorageSharedKeyCredential(storageOptions.AccountName, storageOptions.SharedKey));
        });
    })
    .Build();

host.Run();