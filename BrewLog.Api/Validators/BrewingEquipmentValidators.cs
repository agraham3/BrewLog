using FluentValidation;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators.Extensions;

namespace BrewLog.Api.Validators;

public class CreateBrewingEquipmentDtoValidator : AbstractValidator<CreateBrewingEquipmentDto>
{
    public CreateBrewingEquipmentDtoValidator()
    {
        RuleFor(x => x.Vendor)
            .NotEmpty()
            .WithMessage("Vendor is required")
            .MaximumLength(100)
            .WithMessage("Vendor cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .MaximumLength(100)
            .WithMessage("Model cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .IsValidEnumWithDetails();

        RuleFor(x => x.Specifications)
            .NotNull()
            .WithMessage("Specifications cannot be null");

        RuleFor(x => x.Specifications)
            .Must(specs => specs.Count <= 20)
            .WithMessage("Cannot have more than 20 specifications")
            .When(x => x.Specifications != null);

        RuleFor(x => x.Specifications)
            .Must(BeValidSpecifications)
            .WithMessage("Specification keys and values must not be empty and cannot exceed 100 characters each")
            .When(x => x.Specifications != null);

        // Type-specific validation
        When(x => x.Type == EquipmentType.EspressoMachine && x.Specifications != null, () =>
        {
            RuleFor(x => x.Specifications)
                .Must(ContainValidPressureSpec)
                .WithMessage("Espresso machines should have a valid 'Pressure' specification (e.g., '9 bar', '15 bar')");
        });

        When(x => x.Type == EquipmentType.Grinder && x.Specifications != null, () =>
        {
            RuleFor(x => x.Specifications)
                .Must(ContainValidBurrTypeSpec)
                .WithMessage("Grinders should have a 'BurrType' specification (e.g., 'Ceramic', 'Steel')");
        });
    }

    private static bool BeValidSpecifications(Dictionary<string, string> specifications)
    {
        if (specifications == null) return false;

        return specifications.All(spec =>
            !string.IsNullOrWhiteSpace(spec.Key) &&
            !string.IsNullOrWhiteSpace(spec.Value) &&
            spec.Key.Length <= 100 &&
            spec.Value.Length <= 100);
    }

    private static bool ContainValidPressureSpec(Dictionary<string, string> specifications)
    {
        if (specifications == null) return true;

        if (!specifications.ContainsKey("Pressure"))
            return true; // Optional, but if present should be valid

        var pressure = specifications["Pressure"];
        return !string.IsNullOrWhiteSpace(pressure) && pressure.Contains("bar", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainValidBurrTypeSpec(Dictionary<string, string> specifications)
    {
        if (specifications == null) return true;

        if (!specifications.ContainsKey("BurrType"))
            return true; // Optional, but if present should be valid

        var burrType = specifications["BurrType"];
        var validBurrTypes = new[] { "ceramic", "steel", "conical", "flat" };
        return !string.IsNullOrWhiteSpace(burrType) &&
               validBurrTypes.Any(valid => burrType.Contains(valid, StringComparison.OrdinalIgnoreCase));
    }
}

public class UpdateBrewingEquipmentDtoValidator : AbstractValidator<UpdateBrewingEquipmentDto>
{
    public UpdateBrewingEquipmentDtoValidator()
    {
        RuleFor(x => x.Vendor)
            .NotEmpty()
            .WithMessage("Vendor is required")
            .MaximumLength(100)
            .WithMessage("Vendor cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .MaximumLength(100)
            .WithMessage("Model cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .IsValidEnumWithDetails();

        RuleFor(x => x.Specifications)
            .NotNull()
            .WithMessage("Specifications cannot be null");

        RuleFor(x => x.Specifications)
            .Must(specs => specs.Count <= 20)
            .WithMessage("Cannot have more than 20 specifications")
            .When(x => x.Specifications != null);

        RuleFor(x => x.Specifications)
            .Must(BeValidSpecifications)
            .WithMessage("Specification keys and values must not be empty and cannot exceed 100 characters each")
            .When(x => x.Specifications != null);

        // Type-specific validation
        When(x => x.Type == EquipmentType.EspressoMachine && x.Specifications != null, () =>
        {
            RuleFor(x => x.Specifications)
                .Must(ContainValidPressureSpec)
                .WithMessage("Espresso machines should have a valid 'Pressure' specification (e.g., '9 bar', '15 bar')");
        });

        When(x => x.Type == EquipmentType.Grinder && x.Specifications != null, () =>
        {
            RuleFor(x => x.Specifications)
                .Must(ContainValidBurrTypeSpec)
                .WithMessage("Grinders should have a 'BurrType' specification (e.g., 'Ceramic', 'Steel')");
        });
    }

    private static bool BeValidSpecifications(Dictionary<string, string> specifications)
    {
        if (specifications == null) return false;

        return specifications.All(spec =>
            !string.IsNullOrWhiteSpace(spec.Key) &&
            !string.IsNullOrWhiteSpace(spec.Value) &&
            spec.Key.Length <= 100 &&
            spec.Value.Length <= 100);
    }

    private static bool ContainValidPressureSpec(Dictionary<string, string> specifications)
    {
        if (specifications == null) return true;

        if (!specifications.ContainsKey("Pressure"))
            return true; // Optional, but if present should be valid

        var pressure = specifications["Pressure"];
        return !string.IsNullOrWhiteSpace(pressure) && pressure.Contains("bar", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainValidBurrTypeSpec(Dictionary<string, string> specifications)
    {
        if (specifications == null) return true;

        if (!specifications.ContainsKey("BurrType"))
            return true; // Optional, but if present should be valid

        var burrType = specifications["BurrType"];
        var validBurrTypes = new[] { "ceramic", "steel", "conical", "flat" };
        return !string.IsNullOrWhiteSpace(burrType) &&
               validBurrTypes.Any(valid => burrType.Contains(valid, StringComparison.OrdinalIgnoreCase));
    }
}