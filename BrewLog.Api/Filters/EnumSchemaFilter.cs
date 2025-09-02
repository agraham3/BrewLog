using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrewLog.Api.Filters;

/// <summary>
/// Swagger schema filter to display enum values as strings instead of integers
/// and include enum descriptions in the schema
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "string";
            schema.Format = null;

            var enumNames = Enum.GetNames(context.Type);
            var enumValues = Enum.GetValues(context.Type);

            // Add enum values as strings
            foreach (var enumName in enumNames)
            {
                schema.Enum.Add(new OpenApiString(enumName));
            }

            // Add description with all possible values
            var enumDescriptions = enumNames.Select(name => $"'{name}'");
            schema.Description = $"Possible values: {string.Join(", ", enumDescriptions)}";

            // Set example to the first enum value
            if (enumNames.Length > 0)
            {
                schema.Example = new OpenApiString(enumNames[0]);
            }
        }
    }
}