using Testcontainers.Redis;

namespace TwitPoster.IntegrationTests.Extensions;

public static class RedisContainerExtensions
{
    public static async Task ResetAsync(this RedisContainer redisContainer)
    {
        const string luaScript = "return redis.call('FLUSHDB')";
        
        await redisContainer.ExecScriptAsync(luaScript);
    }
}