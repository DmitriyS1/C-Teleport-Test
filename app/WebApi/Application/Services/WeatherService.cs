using CTeleport.Weather.Api.Application.Responses;
using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;

using WeatherResponse = OneOf.OneOf<CTeleport.Weather.Api.Application.Responses.WeatherInformation, OneOf.Types.Error<string[]>>;

namespace CTeleport.Weather.Api.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherHttpClient _weatherHttpClient;
    private readonly ILogger<WeatherService> _logger;

    /// <inheritdoc />
    public async Task<WeatherResponse> GetWeatherAsync(string zip, string countryCode, long date, CancellationToken cancellationToken = default)
    {
        var cityInformation = await _weatherHttpClient.GetCityInformationAsync(zip, countryCode, cancellationToken);
        if (cityInformation.IsT1) {
            // TODO: Define an error and return correct message            
        }

        // TODO: Store value in cache

        // TODO: Make request for the weather information


        // Return weather information or error
    }
}