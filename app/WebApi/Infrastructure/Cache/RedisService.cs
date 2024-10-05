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
    public async Task<T?> GetAsync<T>(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
    }
}