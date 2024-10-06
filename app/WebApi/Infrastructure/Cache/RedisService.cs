using System.Text.Json;
using CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;
using StackExchange.Redis;

namespace CTeleport.Weather.Api.Infrastructure.Cache;

public class RedisService : IRedisService {
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var db = _connectionMultiplexer.GetDatabase(0);
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : null;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var db = _connectionMultiplexer.GetDatabase(0);
        await db.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
    }
}