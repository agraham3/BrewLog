using System.ComponentModel;
using System.Globalization;

namespace BrewLog.Api.Converters;

/// <summary>
/// Custom type converter for enums that supports both string names and integer values
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
public class EnumTypeConverter<T> : TypeConverter where T : struct, Enum
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return null;

            // Try case-insensitive parsing
            if (Enum.TryParse<T>(stringValue, ignoreCase: true, out var enumValue))
            {
                return enumValue;
            }

            throw new FormatException($"Unable to convert '{stringValue}' to {typeof(T).Name}. Valid values are: {string.Join(", ", Enum.GetNames<T>())}");
        }

        if (value is int intValue)
        {
            if (Enum.IsDefined(typeof(T), intValue))
            {
                return (T)Enum.ToObject(typeof(T), intValue);
            }

            throw new FormatException($"Unable to convert {intValue} to {typeof(T).Name}. Valid values are: {string.Join(", ", Enum.GetNames<T>())}");
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || destinationType == typeof(int) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is T enumValue)
        {
            if (destinationType == typeof(string))
                return enumValue.ToString();

            if (destinationType == typeof(int))
                return Convert.ToInt32(enumValue);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}