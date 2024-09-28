namespace CTeleport.Weather.Api.Core.Configurations;

public class WeatherHttpClientConfiguration
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }

    public int MaxResponseContentBufferSize { get; set; }
}