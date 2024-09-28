namespace CTeleport.Weather.Api.Infrastructure.Http.Responses;

/// <summary>
/// Represents city information response from OpenWeather service
/// </summary>
public class CityInformation
{
    public string Zip { get; set; }

    /// <summary>
    /// City name
    /// </summary>
    public string Name { get; set; }

    public string Country { get; set; }

    /// <summary>
    /// Lattitude
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Longitude
    /// </summary>
    public double Lon { get; set; }
}