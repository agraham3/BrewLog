using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrewLog.Api.Filters;

/// <summary>
/// Swagger parameter filter to properly document enum parameters in query strings
/// and request bodies, showing string values instead of integers
/// </summary>
public class EnumParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var parameterType = context.ParameterInfo?.ParameterType;
        
        // Handle nullable enums
        if (parameterType != null && parameterType.IsGenericType && 
            parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            parameterType = parameterType.GetGenericArguments()[0];
        }

        if (parameterType != null && parameterType.IsEnum)
        {
            parameter.Schema.Enum.Clear();
            parameter.Schema.Type = "string";
            parameter.Schema.Format = null;

            var enumNames = Enum.GetNames(parameterType);
            
            // Add enum values as strings
            foreach (var enumName in enumNames)
            {
                parameter.Schema.Enum.Add(new OpenApiString(enumName));
            }

            // Add description with all possible values
            var enumDescriptions = enumNames.Select(name => $"'{name}'");
            var baseDescription = parameter.Description ?? "";
            parameter.Description = string.IsNullOrEmpty(baseDescription) 
                ? $"Possible values: {string.Join(", ", enumDescriptions)}"
                : $"{baseDescription}. Possible values: {string.Join(", ", enumDescriptions)}";

            // Set example to the first enum value
            if (enumNames.Length > 0)
            {
                parameter.Example = new OpenApiString(enumNames[0]);
            }
        }
    }
}