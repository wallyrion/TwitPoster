namespace TwitPoster.IntegrationTests;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<IntegrationTestWebFactory>
{
    
}