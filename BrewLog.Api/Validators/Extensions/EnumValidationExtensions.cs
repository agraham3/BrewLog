using FluentValidation;
using System.ComponentModel;
using System.Reflection;

namespace BrewLog.Api.Validators.Extensions;

/// <summary>
/// Extensions for enhanced enum validation with detailed error messages
/// </summary>
public static class EnumValidationExtensions
{
    /// <summary>
    /// Validates that the enum value is valid and provides detailed error message with accepted values
    /// </summary>
    public static IRuleBuilderOptions<T, TEnum> IsValidEnumWithDetails<T, TEnum>(this IRuleBuilder<T, TEnum> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => Enum.IsDefined(typeof(TEnum), value))
            .WithMessage($"Invalid {typeof(TEnum).Name}. Accepted values: {GetEnumValuesString<TEnum>()}");
    }

    /// <summary>
    /// Validates that the nullable enum value is valid (if provided) and provides detailed error message
    /// </summary>
    public static IRuleBuilderOptions<T, TEnum?> IsValidEnumWithDetails<T, TEnum>(this IRuleBuilder<T, TEnum?> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => !value.HasValue || Enum.IsDefined(typeof(TEnum), value.Value))
            .WithMessage($"Invalid {typeof(TEnum).Name}. Accepted values: {GetEnumValuesString<TEnum>()}");
    }

    /// <summary>
    /// Gets a formatted string of all enum values for error messages
    /// </summary>
    private static string GetEnumValuesString<TEnum>() where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>()
            .Select(value => GetEnumDisplayName(value))
            .ToArray();
        
        return string.Join(", ", enumValues);
    }

    /// <summary>
    /// Gets the display name for an enum value, using Description attribute if available
    /// </summary>
    private static string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : struct, Enum
    {
        var field = typeof(TEnum).GetField(enumValue.ToString());
        if (field == null) return enumValue.ToString();

        var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? enumValue.ToString();
    }
}

/// <summary>
/// Custom validation messages for common validation scenarios
/// </summary>
public static class ValidationMessages
{
    public static class Temperature
    {
        public const string Range = "Water temperature must be between {MinValue}°C and {MaxValue}°C";
        public const string EspressoRange = "Espresso water temperature should be between 88°C and 96°C for optimal extraction";
        public const string PourOverRange = "Pour over water temperature should be between 90°C and 96°C for best flavor extraction";
        public const string FrenchPressRange = "French press water temperature should be between 92°C and 96°C for proper brewing";
        public const string ColdBrewRange = "Cold brew water temperature should be between 4°C and 25°C (room temperature or cold)";
    }

    public static class Time
    {
        public const string Range = "Brew time must be between {MinValue} and {MaxValue}";
        public const string EspressoRange = "Espresso brew time should be between 20 and 40 seconds for proper extraction";
        public const string PourOverRange = "Pour over brew time should be between 2 and 6 minutes for optimal flavor";
        public const string FrenchPressRange = "French press brew time should be between 3 and 5 minutes for full extraction";
        public const string ColdBrewRange = "Cold brew time should be between 8 and 24 hours for proper extraction";
    }

    public static class Rating
    {
        public const string Range = "Rating must be between 1 and 10, where 1 is poor and 10 is excellent";
    }

    public static class GrindSize
    {
        public const string Range = "Grind size must be between 1 (finest, for espresso) and 30 (coarsest, for cold brew)";
    }

    public static class Weight
    {
        public const string Range = "Weight must be between {MinValue}g and {MaxValue}g";
        public const string GrindWeightRange = "Grind weight must be between 0.1g and 1000g (typical range: 15-30g for most brewing methods)";
    }
}