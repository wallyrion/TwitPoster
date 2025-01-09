using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        var storageConnectionString = context.Configuration.GetValue<string>("AzureWebJobsStorage");
        
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(storageConnectionString);
        });
    })
    .Build();

host.Run();