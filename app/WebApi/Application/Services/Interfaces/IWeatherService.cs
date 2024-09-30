using CTeleport.Weather.Api.Application.Responses;

namespace CTeleport.Weather.Api.Application.Services.Interfaces;

public interface IWeatherService
{
    /// <summary>
    /// Get city information by zip code and country code
    /// </summary>
    /// <param name="zip">Zip code</param>
    /// <param name="countryCode">Country code in ISO3166 format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>City information or Error</returns>
    Task<CityInformation> GetCityInformationAsync(string zip, string countryCode, CancellationToken cancellationToken = default);

    Task<WeatherInformation> GetWeatherInformationAsync(double lat, double lon, long time, CancellationToken cancellationToken = default);
}