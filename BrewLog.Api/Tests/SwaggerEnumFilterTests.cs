using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using BrewLog.Api.Filters;
using BrewLog.Api.Models;
using Xunit;

namespace BrewLog.Api.Tests;

public class SwaggerEnumFilterTests
{
    [Fact]
    public void EnumSchemaFilter_Should_Convert_Enum_To_String_Schema()
    {
        // Arrange
        var filter = new EnumSchemaFilter();
        var schema = new OpenApiSchema
        {
            Type = "integer",
            Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny>()
        };
        var context = new SchemaFilterContext(typeof(RoastLevel), null, null);

        // Act
        filter.Apply(schema, context);

        // Assert
        Assert.Equal("string", schema.Type);
        Assert.Equal(5, schema.Enum.Count); // RoastLevel has 5 values
        
        var enumValues = schema.Enum.Cast<Microsoft.OpenApi.Any.OpenApiString>().Select(e => e.Value).ToList();
        Assert.Contains("Light", enumValues);
        Assert.Contains("Medium", enumValues);
        Assert.Contains("Dark", enumValues);
        Assert.Contains("Possible values:", schema.Description);
    }

    [Fact]
    public void EnumSchemaFilter_Should_Not_Modify_Non_Enum_Schema()
    {
        // Arrange
        var filter = new EnumSchemaFilter();
        var schema = new OpenApiSchema
        {
            Type = "string",
            Description = "Original description"
        };
        var context = new SchemaFilterContext(typeof(string), null, null);

        // Act
        filter.Apply(schema, context);

        // Assert
        Assert.Equal("string", schema.Type);
        Assert.Equal("Original description", schema.Description);
        Assert.Empty(schema.Enum);
    }
}