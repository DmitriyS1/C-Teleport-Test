using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using OneOf.Types;
using WeatherResponse = OneOf.OneOf<CTeleport.Weather.Api.Application.Responses.WeatherInformation, OneOf.Types.Error<string[]>>;

namespace CTeleport.Weather.Api.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IRedisService _redisService;
    private readonly IWeatherHttpClient _weatherHttpClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        ILogger<WeatherService> logger,
        IRedisService redisService,
        IWeatherHttpClient weatherHttpClient)
    {
        _logger = logger;
        _redisService = redisService;
        _weatherHttpClient = weatherHttpClient;
    }

    /// <inheritdoc />
    public async Task<WeatherResponse> GetWeatherAsync(string zip, string countryCode, long date, CancellationToken cancellationToken = default)
    {
        var cityInformation = await _redisService.GetAsync<Infrastructure.Http.Responses.CityInformation>($"{zip}-{countryCode}");
        if (cityInformation is null) {
            var cityResponse = await _weatherHttpClient.GetCityInformationAsync(zip, countryCode, cancellationToken);
            if (cityResponse.IsT1) {
                _logger.LogError("Error while getting city information: {Error}", cityResponse.AsT1.Value.Message);
                return new Error<string[]>(new[] { cityResponse.AsT1.Value.Message });
            }

            cityInformation = cityResponse.AsT0;
            await _redisService.SetAsync($"{zip}-{countryCode}", cityInformation);
        }

        // TODO: Make request for the weather information
        var weatherResponse = await _weatherHttpClient.GetWeatherInformationAsync(cityInformation.Lat, cityInformation.Lon, date, cancellationToken);
        if (weatherResponse.IsT1) {
            _logger.LogError("Error while getting weather information: {Error}", weatherResponse.AsT1.Value.Message);
            return new Error<string[]>(new[] { weatherResponse.AsT1.Value.Message });
        }


        // Return weather information or error
    }
}