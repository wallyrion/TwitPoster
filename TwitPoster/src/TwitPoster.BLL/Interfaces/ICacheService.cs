namespace TwitPoster.BLL.Interfaces;

public interface ICacheService
{
    public Task<T?> GetFromCacheOrCreate<T>(string key, Func<Task<T?>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
}
