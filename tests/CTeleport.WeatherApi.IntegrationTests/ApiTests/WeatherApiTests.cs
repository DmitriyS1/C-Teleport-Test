using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CTeleport.WeatherApi.IntegrationTests.ApiTests;

public class WeatherApiTests : IClassFixture<WebApplicationFactory>
{
    private readonly WebApplicationFactory _factory;

    public WeatherApiTests(WebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWeather_ShouldReturnOk_WithValidData()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/weather?zip=10001&countryCode=US");

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetWeather_ShouldReturn422_WhenInvalidDataProvided()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/weather?zip=&countryCode=US");

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.UnprocessableEntity);
    }
}