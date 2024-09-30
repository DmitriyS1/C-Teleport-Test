namespace CTeleport.Weather.Api.Application.Services.Interfaces;

public interface IWeatherService
{
    Task<WeatherInformation> GetWeatherInformationAsync(string zip, string countryCode, CancellationToken cancellationToken = default);

    Task<CityInformation> GetCityInformationAsync(double lat, double lon, long time, CancellationToken cancellationToken = default);
}