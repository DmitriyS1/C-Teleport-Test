namespace CTeleport.Weather.Api.Application.Responses;

/// <summary>
/// Represents weather information response from OpenWeather service
/// </summary>
public record WeatherInformation(
    double Temperature, 
    double FeelsLike, 
    double Pressure, 
    double Humidity, 
    double WindSpeed, 
    double WindDeg, 
    double Clouds, 
    double Uvi, 
    double Visibility, 
    string State,
    string Description, 
    string Icon, 
    long Time
);