using CTeleport.Weather.Api.Application.Responses;
using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.WebApi.Application.Requests;
using CTeleport.Weather.Api.WebApi.Validators;
using Microsoft.AspNetCore.Mvc;

namespace CTeleport.Weather.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(
        IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    [ProducesResponseType<WeatherInformation>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] WeatherRequest request, CancellationToken cancellationToken)
    {
        var validationResault = new WeatherRequestValidator().Validate(request);
        if (!validationResault.IsValid)
        {
            return UnprocessableEntity(new {errors = validationResault.Errors.Select(e => e.ErrorMessage)});
        }

        var result = await _weatherService.GetWeatherAsync(
            request.Zip, 
            request.CountryCode, 
            new DateTimeOffset(request.Date).ToUnixTimeSeconds(), 
            request.Units.ToString(), 
            cancellationToken
        );

        return result.Match<IActionResult>(
            Ok,
            errors => BadRequest(new { errors = errors.Value})
        );
    }
}
