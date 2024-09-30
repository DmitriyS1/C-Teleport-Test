using CTeleport.Weather.Api.Application.Responses;
using CTeleport.Weather.Api.Application.Services.Interfaces;

namespace CTeleport.Weather.Api.Application.Services;

public class WeatherService : IWeatherService
{
    /// <inheritdoc />
    public Task<CityInformation> GetCityInformationAsync(string zip, string countryCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<WeatherInformation> GetWeatherInformationAsync(double lat, double lon, long time, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}