using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrewLog.Api.Converters;

/// <summary>
/// Custom JSON converter that serializes enums as strings by default
/// while maintaining backward compatibility with integer values on input
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
public class StringEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle string input (case-insensitive)
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                throw new JsonException($"Cannot convert empty string to {typeof(T).Name}");
            }

            // Try case-insensitive parsing
            if (Enum.TryParse<T>(stringValue, ignoreCase: true, out var enumValue))
            {
                return enumValue;
            }

            // If parsing fails, provide helpful error message with valid values
            var validValues = string.Join(", ", Enum.GetNames<T>());
            throw new JsonException($"Unable to convert '{stringValue}' to {typeof(T).Name}. Valid values are: {validValues}");
        }

        // Handle integer input for backward compatibility
        if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(T), intValue))
            {
                return (T)Enum.ToObject(typeof(T), intValue);
            }

            var validValues = string.Join(", ", Enum.GetNames<T>());
            throw new JsonException($"Unable to convert {intValue} to {typeof(T).Name}. Valid values are: {validValues}");
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Always serialize as string name
        writer.WriteStringValue(value.ToString());
    }
}