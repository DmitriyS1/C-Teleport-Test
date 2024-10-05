namespace CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;

/// <summary>
/// Redis service
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Gets value from cache
    /// </summary>
    /// <typeparam name="T">Gets value of type T from cache</typeparam>
    /// <param name="key">Key to get object from cache</param>
    /// <returns>Value of type T</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Sets value to cache
    /// </summary>
    /// <typeparam name="T">Sets value of type T to cache</typeparam>
    /// <param name="key">Key for the object</param>
    /// <param name="value">Data to store in cache</param>
    /// <param name="expiry">How long to store the value</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
}