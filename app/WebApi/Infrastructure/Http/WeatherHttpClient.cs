using System.Text.Json;
using CTeleport.Weather.Api.Core.Configurations;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Responses;
using Microsoft.Extensions.Options;
using OneOf.Types;
using CityInformation = OneOf.OneOf<CTeleport.Weather.Api.Infrastructure.Http.Responses.CityInformation, OneOf.Types.Error<CTeleport.Weather.Api.Infrastructure.Http.Responses.ErrorResponse>>;
using WeatherInformation = OneOf.OneOf<CTeleport.Weather.Api.Infrastructure.Http.Responses.WeatherInformation, OneOf.Types.Error<CTeleport.Weather.Api.Infrastructure.Http.Responses.ErrorResponse>>;

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
    public async Task<CityInformation> GetCityInformationAsync(string zip, string countryCode, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"geo/1.0/zip?zip={zip},{countryCode}&appid={_configuration.ApiKey}", cancellationToken);
        if (result.IsSuccessStatusCode) {
            var content = await result.Content.ReadAsStringAsync(cancellationToken);
            var cityInformationResult = JsonSerializer.Deserialize<Responses.CityInformation>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cityInformationResult;
        }

        return new Error<ErrorResponse>(new ErrorResponse
        {
            StatusCode = result.StatusCode,
            Message = result.ReasonPhrase
        });
    }

    /// <inheritdoc />
    public async Task<WeatherInformation> GetWeatherInformationAsync(double lat, double lon, long time, string units, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"data/3.0/onecall/timemachine?lat={lat}&lon={lon}&dt={time}&appid={_configuration.ApiKey}", cancellationToken);
        if (result.IsSuccessStatusCode) {
            var content = await result.Content.ReadAsStringAsync(cancellationToken);
            var weatherInformationResult = JsonSerializer.Deserialize<Responses.WeatherInformation>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return weatherInformationResult;
        }

        return new Error<ErrorResponse>(new ErrorResponse
        {
            StatusCode = result.StatusCode,
            Message = result.ReasonPhrase
        });
    }
}