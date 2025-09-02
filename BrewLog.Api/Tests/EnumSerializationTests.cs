using System.Text.Json;
using BrewLog.Api.Converters;
using BrewLog.Api.Models;
using Xunit;

namespace BrewLog.Api.Tests;

public class EnumSerializationTests
{
    private readonly JsonSerializerOptions _options;

    public EnumSerializationTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters = { new StringEnumConverter<RoastLevel>() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public void Should_Serialize_Enum_As_String()
    {
        // Arrange
        var roastLevel = RoastLevel.Medium;

        // Act
        var json = JsonSerializer.Serialize(roastLevel, _options);

        // Assert
        Assert.Equal("\"Medium\"", json);
    }

    [Fact]
    public void Should_Deserialize_String_To_Enum()
    {
        // Arrange
        var json = "\"Medium\"";

        // Act
        var roastLevel = JsonSerializer.Deserialize<RoastLevel>(json, _options);

        // Assert
        Assert.Equal(RoastLevel.Medium, roastLevel);
    }

    [Fact]
    public void Should_Deserialize_String_Case_Insensitive()
    {
        // Arrange
        var json = "\"medium\"";

        // Act
        var roastLevel = JsonSerializer.Deserialize<RoastLevel>(json, _options);

        // Assert
        Assert.Equal(RoastLevel.Medium, roastLevel);
    }

    [Fact]
    public void Should_Deserialize_Integer_For_Backward_Compatibility()
    {
        // Arrange
        var json = "2"; // Medium = 2

        // Act
        var roastLevel = JsonSerializer.Deserialize<RoastLevel>(json, _options);

        // Assert
        Assert.Equal(RoastLevel.Medium, roastLevel);
    }

    [Fact]
    public void Should_Throw_Exception_For_Invalid_String()
    {
        // Arrange
        var json = "\"InvalidValue\"";

        // Act & Assert
        var exception = Assert.Throws<JsonException>(() => 
            JsonSerializer.Deserialize<RoastLevel>(json, _options));
        
        Assert.Contains("Unable to convert 'InvalidValue' to RoastLevel", exception.Message);
        Assert.Contains("Valid values are:", exception.Message);
    }

    [Fact]
    public void Should_Throw_Exception_For_Invalid_Integer()
    {
        // Arrange
        var json = "99"; // Invalid enum value

        // Act & Assert
        var exception = Assert.Throws<JsonException>(() => 
            JsonSerializer.Deserialize<RoastLevel>(json, _options));
        
        Assert.Contains("Unable to convert 99 to RoastLevel", exception.Message);
    }
}