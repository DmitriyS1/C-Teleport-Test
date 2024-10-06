using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CTeleport.Weather.Api.Application.Services;
using CTeleport.Weather.Api.Application.Services.Interfaces;
using CTeleport.Weather.Api.Core.Configurations;
using CTeleport.Weather.Api.Infrastructure.Cache;
using CTeleport.Weather.Api.Infrastructure.Cache.Interfaces;
using CTeleport.Weather.Api.Infrastructure.Http;
using CTeleport.Weather.Api.Infrastructure.Http.Interfaces;
using CTeleport.Weather.Api.Core.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using StackExchange.Redis;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "CTELEPORT_WEATHER_API_");
// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "CTeleport Weather API")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// Rate limiter should prevent DoS attacks especially when we don't have any authentication and authorization.
var rateLimiterSettings = GetRateLimiterSettings(builder.Configuration);
builder.Services.AddRateLimiter(opts => opts.AddFixedWindowLimiter(policyName: "default", limiterOpts => {
    limiterOpts.PermitLimit = (int)rateLimiterSettings.PermitLimit;
    limiterOpts.Window = TimeSpan.FromSeconds(rateLimiterSettings.WindowInSeconds);
    limiterOpts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    limiterOpts.QueueLimit = (int)rateLimiterSettings.QueueLimit;
}));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddHealthChecks().AddRedis(builder.Configuration.GetSection("Redis")["ConnectionString"]);
builder.Services.UseHttpClientMetrics();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSwaggerGen();

builder.Services.Configure<WeatherHttpClientConfiguration>(builder.Configuration.GetSection(nameof(WeatherHttpClientConfiguration)));
builder.Services.AddHttpClient<IWeatherHttpClient, WeatherHttpClient>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetSection("Redis")["ConnectionString"]));
builder.Services.AddSingleton<IRedisService, RedisService>();

builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRateLimiter();
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers().RequireRateLimiting("default");
app.MapHealthChecks("/_health");
app.UseMetricServer("/_metrics");
app.UseHttpMetrics();

app.Run();


static RateLimiterSettings GetRateLimiterSettings(IConfiguration configuration)
{
    var rateLimiterSettings = configuration.GetSection(nameof(RateLimiterSettings)).Get<RateLimiterSettings>();
    if (rateLimiterSettings is null)
    {
        throw new ArgumentNullException(nameof(rateLimiterSettings));
    }

    if (rateLimiterSettings.PermitLimit <= 0 || rateLimiterSettings.WindowInSeconds <= 0 || rateLimiterSettings.QueueLimit <= 0)
    {
        throw new ArgumentOutOfRangeException(nameof(rateLimiterSettings.PermitLimit));
    }

    return rateLimiterSettings;
}