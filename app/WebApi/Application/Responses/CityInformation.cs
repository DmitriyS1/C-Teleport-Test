namespace CTeleport.Weather.Api.Application.Responses;

/// <summary>
/// Represents city information response from OpenWeather service
/// </summary>
/// <param name="Zip">Zip code</param>
/// <param name="Name">City's name</param>
/// <param name="Country">City's country</param>
/// <param name="Lat">Lattitude</param>
/// <param name="Lon">Longitude</param>
public record CityInformation(
    string Zip,
    string Name,
    string Country,
    double Lat,
    double Lon
);