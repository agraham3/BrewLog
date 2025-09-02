using FluentValidation;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators.Extensions;

namespace BrewLog.Api.Validators;

public class CreateBrewSessionDtoValidator : AbstractValidator<CreateBrewSessionDto>
{
    public CreateBrewSessionDtoValidator()
    {
        RuleFor(x => x.Method)
            .IsValidEnumWithDetails();

        RuleFor(x => x.WaterTemperature)
            .GreaterThan(0)
            .WithMessage("Water temperature must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Water temperature cannot exceed 100°C");

        // Method-specific temperature validation
        When(x => x.Method == BrewMethod.Espresso, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(88, 96)
                .WithMessage(ValidationMessages.Temperature.EspressoRange);
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(90, 96)
                .WithMessage(ValidationMessages.Temperature.PourOverRange);
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(92, 96)
                .WithMessage(ValidationMessages.Temperature.FrenchPressRange);
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(4, 25)
                .WithMessage(ValidationMessages.Temperature.ColdBrewRange);
        });

        RuleFor(x => x.BrewTime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Brew time must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromHours(24))
            .WithMessage("Brew time cannot exceed 24 hours");

        // Method-specific time validation
        When(x => x.Method == BrewMethod.Espresso, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40))
                .WithMessage(ValidationMessages.Time.EspressoRange);
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(6))
                .WithMessage(ValidationMessages.Time.PourOverRange);
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5))
                .WithMessage(ValidationMessages.Time.FrenchPressRange);
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromHours(8), TimeSpan.FromHours(24))
                .WithMessage(ValidationMessages.Time.ColdBrewRange);
        });

        RuleFor(x => x.TastingNotes)
            .MaximumLength(1000)
            .WithMessage("Tasting notes cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .When(x => x.Rating.HasValue)
            .WithMessage(ValidationMessages.Rating.Range);

        RuleFor(x => x.CoffeeBeanId)
            .GreaterThan(0)
            .WithMessage("Coffee bean ID must be greater than 0");

        RuleFor(x => x.GrindSettingId)
            .GreaterThan(0)
            .WithMessage("Grind setting ID must be greater than 0");

        RuleFor(x => x.BrewingEquipmentId)
            .GreaterThan(0)
            .When(x => x.BrewingEquipmentId.HasValue)
            .WithMessage("Brewing equipment ID must be greater than 0");
    }
}

public class UpdateBrewSessionDtoValidator : AbstractValidator<UpdateBrewSessionDto>
{
    public UpdateBrewSessionDtoValidator()
    {
        RuleFor(x => x.Method)
            .IsValidEnumWithDetails();

        RuleFor(x => x.WaterTemperature)
            .GreaterThan(0)
            .WithMessage("Water temperature must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Water temperature cannot exceed 100°C");

        // Method-specific temperature validation
        When(x => x.Method == BrewMethod.Espresso, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(88, 96)
                .WithMessage(ValidationMessages.Temperature.EspressoRange);
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(90, 96)
                .WithMessage(ValidationMessages.Temperature.PourOverRange);
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(92, 96)
                .WithMessage(ValidationMessages.Temperature.FrenchPressRange);
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(4, 25)
                .WithMessage(ValidationMessages.Temperature.ColdBrewRange);
        });

        RuleFor(x => x.BrewTime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Brew time must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromHours(24))
            .WithMessage("Brew time cannot exceed 24 hours");

        // Method-specific time validation
        When(x => x.Method == BrewMethod.Espresso, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40))
                .WithMessage(ValidationMessages.Time.EspressoRange);
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(6))
                .WithMessage(ValidationMessages.Time.PourOverRange);
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5))
                .WithMessage(ValidationMessages.Time.FrenchPressRange);
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromHours(8), TimeSpan.FromHours(24))
                .WithMessage(ValidationMessages.Time.ColdBrewRange);
        });

        RuleFor(x => x.TastingNotes)
            .MaximumLength(1000)
            .WithMessage("Tasting notes cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .When(x => x.Rating.HasValue)
            .WithMessage(ValidationMessages.Rating.Range);

        RuleFor(x => x.CoffeeBeanId)
            .GreaterThan(0)
            .WithMessage("Coffee bean ID must be greater than 0");

        RuleFor(x => x.GrindSettingId)
            .GreaterThan(0)
            .WithMessage("Grind setting ID must be greater than 0");

        RuleFor(x => x.BrewingEquipmentId)
            .GreaterThan(0)
            .When(x => x.BrewingEquipmentId.HasValue)
            .WithMessage("Brewing equipment ID must be greater than 0");
    }
}