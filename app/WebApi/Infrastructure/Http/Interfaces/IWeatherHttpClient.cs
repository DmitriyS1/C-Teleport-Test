using CTeleport.Weather.Api.Infrastructure.Http.Responses;

namespace CTeleport.Weather.Api.Infrastructure.Http.Interfaces;

public interface IWeatherHttpClient {

    /// <summary>
    /// Returns the city information for the given zip code.
    /// </summary>
    /// <param name="zip">Zip code</param>
    /// <param name="countryCode">Country code in ISO3166 format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Data about the city <see cref="CityInformation"/></returns>
    Task<CityInformation> GetCityInformationAsync(string zip, string countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the weather information for the given coordinates.
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="time">Time in unix format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather information <see cref="WeatherInformation"/></returns>
    Task<WeatherInformation> GetWeatherInformationAsync(double lat, double lon, long time, CancellationToken cancellationToken = default);
}