using WeatherResponse = OneOf.OneOf<CTeleport.Weather.Api.Application.Responses.WeatherInformation, OneOf.Types.Error<string[]>>;

namespace CTeleport.Weather.Api.Application.Services.Interfaces;

public interface IWeatherService
{
    /// <summary>
    /// Get weather information by coordinates
    /// </summary>    
    /// <param name="zip">Zip code</param>
    /// <param name="countryCode">Country code in ISO3166 format</param>
    /// <param name="date">Date in unix format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather information or Error</returns>
    Task<WeatherResponse> GetWeatherAsync(string zip, string countryCode, long date, CancellationToken cancellationToken = default);
}