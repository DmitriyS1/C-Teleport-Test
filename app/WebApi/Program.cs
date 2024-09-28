using System.Threading.RateLimiting;
using CTeleport.Weather.Api.Core.Configurations;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Rate limiter should prevent DoS attacks especially when we don't have any authentication and authorization.
var rateLimiterSettings = GetRateLimiterSettings(builder.Configuration);
builder.Services.AddRateLimiter(opts => opts.AddFixedWindowLimiter(policyName: "default", limiterOpts => {
    limiterOpts.PermitLimit = (int)rateLimiterSettings.PermitLimit;
    limiterOpts.Window = TimeSpan.FromSeconds(rateLimiterSettings.WindowInSeconds);
    limiterOpts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    limiterOpts.QueueLimit = (int)rateLimiterSettings.QueueLimit;
}));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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