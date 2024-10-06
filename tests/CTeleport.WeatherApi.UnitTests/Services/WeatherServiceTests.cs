using CTeleport.Weather.Api.Application.Responses;
using CTeleport.Weather.Api.Application.Services;
using CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf;
using OneOf.Types;

namespace CTeleport.WeatherApi.UnitTests.Services;

public class WeatherServiceTests {
    private readonly Mock<ILogger<WeatherService>> _logger;

    private readonly Mock<IWeatherHttpClient> _weatherHttpClientMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests() {
        _logger = new Mock<ILogger<WeatherService>>();
        _weatherHttpClientMock = new Mock<IWeatherHttpClient>();
        _redisServiceMock = new Mock<IRedisService>();
        _weatherService = new WeatherService(_logger.Object, _redisServiceMock.Object, _weatherHttpClientMock.Object);
    }

    [Fact]
    public async Task Given_ValidRequest_When_GetWeatherAsync_Then_ReturnsWeatherInformation() {
        // Arrange
        var countryCode = "US";
        var date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var measureUnit = "standard";
        var cancellationToken = CancellationToken.None;

        var weatherResponse = new Weather.Api.Infrastructure.Http.Responses.WeatherInformation(){
            Data = new WeatherData(){
                Temp = 26.7,
                FeelsLike = 26,
                Humidity = 125,
                WindSpeed = 12,
                WindDeg = 3,
                Clouds = 7,
                Uvi = 1.86,
                Visibility = 1300,
                Weather = [
                    new InternalWeather(){
                        Main = "Sunny",
                        Description = "Sunny day",
                        Icon = "Icon"
                    }
                ],
                DT = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };
        _redisServiceMock.Setup(x => x.GetAsync<CityInformation>(It.IsAny<string>()))
            .ReturnsAsync((CityInformation)null);
        _redisServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CityInformation>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);
        _weatherHttpClientMock.Setup(x => x.GetCityInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CityInformation { Lat = 12.3, Lon = 45.6, Country = countryCode, Name = "City" });
        _weatherHttpClientMock.Setup(x => x.GetWeatherInformationAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await _weatherService.GetWeatherAsync("zip", countryCode, date, measureUnit, cancellationToken);

        // Assert
        Assert.IsType<Weather.Api.Application.Responses.WeatherInformation>(result.Value);
        Assert.True(result.IsT0);
        _redisServiceMock.Verify(x => x.GetAsync<CityInformation>(It.IsAny<string>()), Times.Once);
        _weatherHttpClientMock.Verify(x => x.GetCityInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _weatherHttpClientMock.Verify(x => x.GetWeatherInformationAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_ErrorFromHttpClientGetWeather_When_GetWeatherAsync_Then_ReturnsError() {
        // Arrange
        var zip = "12345";
        var countryCode = "US";
        var date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var measureUnit = "standard";
        var cancellationToken = CancellationToken.None;

        var cityInformation = new CityInformation { Lat = 12.3, Lon = 45.6, Country = countryCode, Name = "City" };

        var errorResponse = new ErrorResponse{ Message = "Error message" };
        var weatherResponse = new Error<ErrorResponse>(errorResponse);
        _redisServiceMock.Setup(x => x.GetAsync<CityInformation>(It.IsAny<string>()))
            .ReturnsAsync(cityInformation);
        _weatherHttpClientMock.Setup(x => x.GetWeatherInformationAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await _weatherService.GetWeatherAsync(zip, countryCode, date, measureUnit, cancellationToken);

        // Assert
        Assert.IsType<Error<string[]>>(result.Value);
        _redisServiceMock.Verify(x => x.GetAsync<CityInformation>(It.IsAny<string>()), Times.Once);
        _weatherHttpClientMock.Verify(x => x.GetWeatherInformationAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_ErrorFromHttpClientGetCityInformation_When_GetWeatherAsync_Then_ReturnsError() {
        // Arrange
        var zip = "12345";
        var countryCode = "US";
        var date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var measureUnit = "standard";
        var cancellationToken = CancellationToken.None;

        var errorResponse = new ErrorResponse{ Message = "Error message" };
        var cityResponse = new Error<ErrorResponse>(errorResponse);
        _redisServiceMock.Setup(x => x.GetAsync<CityInformation>(It.IsAny<string>()))
            .ReturnsAsync((CityInformation)null);
        _weatherHttpClientMock.Setup(x => x.GetCityInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cityResponse);

        // Act
        var result = await _weatherService.GetWeatherAsync(zip, countryCode, date, measureUnit, cancellationToken);

        // Assert
        Assert.IsType<Error<string[]>>(result.Value);
        _redisServiceMock.Verify(x => x.GetAsync<CityInformation>(It.IsAny<string>()), Times.Once);
        _weatherHttpClientMock.Verify(x => x.GetCityInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}