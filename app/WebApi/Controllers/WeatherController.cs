using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CTeleport.Weather.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(
        ILogger<WeatherController> logger,
        IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] WeatherRequest request, CancellationToken cancellationToken)
    {
        var result = await _weatherService.GetWeatherAsync("zip", "countryCode", 0, "measureUnit", cancellationToken);
        return result.Match<IActionResult>(
            Ok,
            errors => UnprocessableEntity(errors.Value));
    }
}
