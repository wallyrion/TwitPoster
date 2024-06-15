using Azure.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var section = context.Configuration.GetRequiredSection("Storage");
        var storageOptions = section.Get<StorageOptions>();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        Console.WriteLine("storage options are  " + storageOptions.Uri + " " + storageOptions.AccountName + " " + storageOptions.SharedKey);
        
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(new Uri(storageOptions.Uri), new StorageSharedKeyCredential(storageOptions.AccountName, storageOptions.SharedKey));
        });
    })
    .Build();

host.Run();


public class StorageOptions
{
    public string Uri { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string SharedKey { get; set; } = null!;
}

