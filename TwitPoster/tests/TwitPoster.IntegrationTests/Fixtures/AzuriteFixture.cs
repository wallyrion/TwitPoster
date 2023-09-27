using Testcontainers.Azurite;

namespace TwitPoster.IntegrationTests.Fixtures;

public class AzuriteFixture : IAsyncLifetime
{
    public const string AccountName = "devstoreaccount1";
    public const string SharedKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
    private const string BlobPort = "10000";
    private ushort _blobPublicPort;
    private string Host => _azuriteContainer.Hostname;
    public string Uri => $"http://{Host}:{_blobPublicPort}/{AccountName}";
    
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _azuriteContainer.StartAsync();
        var publicPort = _azuriteContainer.GetMappedPublicPort(BlobPort);
        _blobPublicPort = publicPort;
    }

    public async Task DisposeAsync()
    {
        await _azuriteContainer.DisposeAsync();
    }
}