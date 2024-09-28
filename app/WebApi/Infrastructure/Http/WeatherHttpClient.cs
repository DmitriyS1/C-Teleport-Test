using CTeleport.Weather.Api.Core.Configurations;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Responses;
using Microsoft.Extensions.Options;

namespace CTeleport.Weather.Api.Infrastructure.Http;

public class WeatherHttpClient : IWeatherHttpClient
{
    private readonly WeatherHttpClientConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    public WeatherHttpClient(
        HttpClient httpClient,
        IOptions<WeatherHttpClientConfiguration> configuration)
    {
        _configuration = configuration.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.MaxResponseContentBufferSize = _configuration.MaxResponseContentBufferSize;
    }
    
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