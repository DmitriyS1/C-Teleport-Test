namespace CTeleport.Weather.Api.Infrastructure.Http.Responses;

public class WeatherInformation
{
    public double Lat { get; set; }

    public double Lon { get; set; }

    public string Timezone { get; set; }

    public long TimezoneOffset { get; set; }

    public WeatherData Data { get; set; }

}

public class WeatherData {
    public long DT { get; set; }
    public long Sunrise { get; set; }
    public long Sunset { get; set; }

    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Pressure { get; set; }

    public int Humidity { get; set; }
    public double DewPoint { get; set; }

    public double Uvi { get; set; }
    public int Clouds { get; set; }
    public int Visibility { get; set; }

    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }

    public InternalWeather[] Weather { get; set; }
}

public class InternalWeather {
    public int Id { get; set; }
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}
