using FluentValidation;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;

namespace BrewLog.Api.Validators;

public class CreateBrewSessionDtoValidator : AbstractValidator<CreateBrewSessionDto>
{
    public CreateBrewSessionDtoValidator()
    {
        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Invalid brew method");

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
                .WithMessage("Espresso water temperature should be between 88°C and 96°C");
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(90, 96)
                .WithMessage("Pour over water temperature should be between 90°C and 96°C");
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(92, 96)
                .WithMessage("French press water temperature should be between 92°C and 96°C");
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(4, 25)
                .WithMessage("Cold brew water temperature should be between 4°C and 25°C");
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
                .WithMessage("Espresso brew time should be between 20 and 40 seconds");
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(6))
                .WithMessage("Pour over brew time should be between 2 and 6 minutes");
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5))
                .WithMessage("French press brew time should be between 3 and 5 minutes");
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromHours(8), TimeSpan.FromHours(24))
                .WithMessage("Cold brew time should be between 8 and 24 hours");
        });

        RuleFor(x => x.TastingNotes)
            .MaximumLength(1000)
            .WithMessage("Tasting notes cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 10");

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
            .IsInEnum()
            .WithMessage("Invalid brew method");

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
                .WithMessage("Espresso water temperature should be between 88°C and 96°C");
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(90, 96)
                .WithMessage("Pour over water temperature should be between 90°C and 96°C");
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(92, 96)
                .WithMessage("French press water temperature should be between 92°C and 96°C");
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.WaterTemperature)
                .InclusiveBetween(4, 25)
                .WithMessage("Cold brew water temperature should be between 4°C and 25°C");
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
                .WithMessage("Espresso brew time should be between 20 and 40 seconds");
        });

        When(x => x.Method == BrewMethod.PourOver, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(6))
                .WithMessage("Pour over brew time should be between 2 and 6 minutes");
        });

        When(x => x.Method == BrewMethod.FrenchPress, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5))
                .WithMessage("French press brew time should be between 3 and 5 minutes");
        });

        When(x => x.Method == BrewMethod.ColdBrew, () =>
        {
            RuleFor(x => x.BrewTime)
                .InclusiveBetween(TimeSpan.FromHours(8), TimeSpan.FromHours(24))
                .WithMessage("Cold brew time should be between 8 and 24 hours");
        });

        RuleFor(x => x.TastingNotes)
            .MaximumLength(1000)
            .WithMessage("Tasting notes cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 10");

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