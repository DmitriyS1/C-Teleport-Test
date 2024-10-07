using CTeleport.Weather.Api.Application.Responses;
using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.Controllers;
using CTeleport.Weather.Api.WebApi.Application.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OneOf.Types;

namespace CTeleport.WeatherApi.UnitTests.Controllers;

public class WeatherControllerTests {
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly WeatherController _weatherController;

    public WeatherControllerTests() {
        _weatherServiceMock = new Mock<IWeatherService>();
        _weatherController = new WeatherController(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Given_ValidRequest_When_Get_Then_ReturnsOkResult() {
        // Arrange
        var weatherRequest = new WeatherRequest {
            Zip = "12345",
            CountryCode = "US",
            Date = DateTime.UtcNow,
            Units = MeasureUnitsEnum.Standard
        };

        var cancellationToken = CancellationToken.None;
        var weatherResult = new WeatherInformation(26.7, 26, 125, 12, 3, 43, 7, 1.86, 1300, "Sunny", "Sunny day", "Icon", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherResult);

        // Act
        var result = await _weatherController.Get(weatherRequest, cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(null, "US", true, MeasureUnitsEnum.Standard)]
    [InlineData("12345", null, true, MeasureUnitsEnum.Standard)]
    [InlineData("12345", "US", false, MeasureUnitsEnum.Standard)]
    [InlineData("12345", "US", true, null)]
    public async Task Given_InvalidRequest_When_Get_Then_ReturnsUnprocessableEntityResult(string? zip, string? countryCode, bool isValidDate, MeasureUnitsEnum? units) {
        // Arrange
        var weatherRequest = new WeatherRequest {
            Zip = zip,
            CountryCode = countryCode,
            Date = isValidDate ? DateTime.UtcNow : new DateTime(1950, 01, 01),
            Units = units
        };

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _weatherController.Get(weatherRequest, cancellationToken);

        // Assert
        var res = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.NotNull(res.Value);
        Assert.Equal("errors", res.Value.GetType().GetProperty("errors")?.Name);
        Assert.NotEmpty((IEnumerable<string>)res.Value.GetType().GetProperty("errors")?.GetValue(res.Value));
    }

    [Fact]
    public async Task Given_ValidRequestAndServiceFailed_When_Get_Then_ReturnsBadRequestResult() {
        // Arrange
        var weatherRequest = new WeatherRequest {
            Zip = "12345",
            CountryCode = "US",
            Date = DateTime.UtcNow,
            Units = MeasureUnitsEnum.Standard
        };

        var cancellationToken = CancellationToken.None;

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Error<string[]>(["Error message"]));

        // Act
        var result = await _weatherController.Get(weatherRequest, cancellationToken);

        // Assert
        var res = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(res.Value);
        Assert.Equal("errors", res.Value.GetType().GetProperty("errors")?.Name);
        Assert.NotEmpty((IEnumerable<string>)res.Value.GetType().GetProperty("errors")?.GetValue(res.Value));
    }
}