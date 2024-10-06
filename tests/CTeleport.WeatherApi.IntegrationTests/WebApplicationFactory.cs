using System.Threading.RateLimiting;
using CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;

namespace CTeleport.WeatherApi.IntegrationTests
{
    public class WebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var logger = scopedServices.GetRequiredService<ILogger<WebApplicationFactory>>();
                logger.LogInformation("Starting integration tests");

                var redisService = new Mock<IRedisService>();
                redisService.Setup(x => x.GetAsync<CityInformation>(It.IsAny<string>()))
                    .ReturnsAsync((CityInformation)null);
                redisService.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CityInformation>(), It.IsAny<TimeSpan>()))
                    .Returns(Task.CompletedTask);

                services.AddSingleton(redisService.Object);

                var weatherHttpClient = new Mock<IWeatherHttpClient>();
                weatherHttpClient.Setup(x => x.GetCityInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(new CityInformation { Lat = 12.3, Lon = 45.6, Country = "US", Name = "City" });

                weatherHttpClient.Setup(x => x.GetWeatherInformationAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(new WeatherInformation
                    {
                        Data = new WeatherData
                        {
                            Temp = 26.7,
                            FeelsLike = 26,
                            Humidity = 125,
                            WindSpeed = 12,
                            WindDeg = 3,
                            Clouds = 7,
                            Uvi = 1.86,
                            Visibility = 1300,
                            Weather = new[]
                            {
                                new InternalWeather
                                {
                                    Main = "Sunny",
                                    Description = "Sunny day",
                                    Icon = "Icon"
                                }
                            },
                            DT = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    });

                services.AddSingleton(weatherHttpClient.Object);
            });
        }
    }
}