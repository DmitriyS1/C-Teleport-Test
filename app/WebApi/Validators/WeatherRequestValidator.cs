namespace CTeleport.Weather.Api.WebApi.Validators;

using CTeleport.Weather.Api.WebApi.Requests;
using FluentValidation;

public class WeatherRequestValidator : AbstractValidator<WeatherRequest>
{
    private const int DAYS_AHEAD = 4;
    private DateTime MIN_DATE = new DateTime(1979, 1, 1);

    public WeatherRequestValidator()
    {
        RuleFor(x => x.Zip)
            .NotEmpty()
            .WithMessage("Zip code is required");

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .Must(x => x > MIN_DATE)
            .WithMessage($"Date should be greater than {MIN_DATE}")
            .Must(x => x < DateTime.Now + TimeSpan.FromDays(DAYS_AHEAD))
            .WithMessage($"Date should be less than {DAYS_AHEAD} days from now");

        RuleFor(x => x.Units)
            .IsInEnum()
            .WithMessage("Invalid units");
    }
}