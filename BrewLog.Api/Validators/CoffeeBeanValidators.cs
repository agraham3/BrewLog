using FluentValidation;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators.Extensions;

namespace BrewLog.Api.Validators;

public class CreateCoffeeBeanDtoValidator : AbstractValidator<CreateCoffeeBeanDto>
{
    public CreateCoffeeBeanDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Coffee bean name is required")
            .MaximumLength(100)
            .WithMessage("Coffee bean name cannot exceed 100 characters");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Brand is required")
            .MaximumLength(100)
            .WithMessage("Brand cannot exceed 100 characters");

        RuleFor(x => x.RoastLevel)
            .IsValidEnumWithDetails();

        RuleFor(x => x.Origin)
            .NotEmpty()
            .WithMessage("Origin is required")
            .MaximumLength(200)
            .WithMessage("Origin cannot exceed 200 characters");
    }
}

public class UpdateCoffeeBeanDtoValidator : AbstractValidator<UpdateCoffeeBeanDto>
{
    public UpdateCoffeeBeanDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Coffee bean name is required")
            .MaximumLength(100)
            .WithMessage("Coffee bean name cannot exceed 100 characters");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Brand is required")
            .MaximumLength(100)
            .WithMessage("Brand cannot exceed 100 characters");

        RuleFor(x => x.RoastLevel)
            .IsValidEnumWithDetails();

        RuleFor(x => x.Origin)
            .NotEmpty()
            .WithMessage("Origin is required")
            .MaximumLength(200)
            .WithMessage("Origin cannot exceed 200 characters");
    }
}