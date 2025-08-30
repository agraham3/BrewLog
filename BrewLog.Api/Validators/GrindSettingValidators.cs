using FluentValidation;
using BrewLog.Api.DTOs;

namespace BrewLog.Api.Validators;

public class CreateGrindSettingDtoValidator : AbstractValidator<CreateGrindSettingDto>
{
    public CreateGrindSettingDtoValidator()
    {
        RuleFor(x => x.GrindSize)
            .InclusiveBetween(1, 30)
            .WithMessage("Grind size must be between 1 and 30");

        RuleFor(x => x.GrindTime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Grind time must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromMinutes(10))
            .WithMessage("Grind time cannot exceed 10 minutes");

        RuleFor(x => x.GrindWeight)
            .GreaterThan(0)
            .WithMessage("Grind weight must be greater than zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Grind weight cannot exceed 1000 grams");

        RuleFor(x => x.GrinderType)
            .NotEmpty()
            .WithMessage("Grinder type is required")
            .MaximumLength(100)
            .WithMessage("Grinder type cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}

public class UpdateGrindSettingDtoValidator : AbstractValidator<UpdateGrindSettingDto>
{
    public UpdateGrindSettingDtoValidator()
    {
        RuleFor(x => x.GrindSize)
            .InclusiveBetween(1, 30)
            .WithMessage("Grind size must be between 1 and 30");

        RuleFor(x => x.GrindTime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Grind time must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromMinutes(10))
            .WithMessage("Grind time cannot exceed 10 minutes");

        RuleFor(x => x.GrindWeight)
            .GreaterThan(0)
            .WithMessage("Grind weight must be greater than zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Grind weight cannot exceed 1000 grams");

        RuleFor(x => x.GrinderType)
            .NotEmpty()
            .WithMessage("Grinder type is required")
            .MaximumLength(100)
            .WithMessage("Grinder type cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}