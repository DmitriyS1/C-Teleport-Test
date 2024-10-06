namespace CTeleport.Weather.Api.WebApi.Application.Requests;


/// <summary>
/// Weather request
/// </summary>
public class WeatherRequest {
    public string Zip { get; set; }
    public string CountryCode { get; set; }

    public DateTime Date { get; set; }

    public MeasureUnitsEnum Units { get; set; }
}