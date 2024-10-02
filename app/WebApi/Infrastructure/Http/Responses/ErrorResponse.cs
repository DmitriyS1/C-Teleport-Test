using System.Net;

namespace CTeleport.Weather.Api.Infrastructure.Http.Responses;

/// <summary>
/// Represents error response from OpenWeather http client
/// </summary>
public class ErrorResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
}