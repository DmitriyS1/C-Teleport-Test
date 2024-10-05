namespace CTeleport.Weather.Api.WebApi.Validators;

using CTeleport.Weather.Api.WebApi.Requests;
using FluentValidation;

public class WeatherRequestValidator : AbstractValidator<WeatherRequest>
{
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
            .Must(x => x > new DateTime(1979, 1, 1))
            .WithMessage("Date should be greater than 01/01/1979")
            .Must(x => x < DateTime.Now + TimeSpan.FromDays(4))
            .WithMessage("Date should be less than 4 days from now");

        RuleFor(x => x.Units)
            .IsInEnum()
            .WithMessage("Invalid units");
    }
}