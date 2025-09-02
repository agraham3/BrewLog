using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using BrewLog.Api.Examples;

namespace BrewLog.Api.Attributes;

/// <summary>
/// Attribute to specify response examples for Swagger documentation
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseExampleAttribute : Attribute
{
    public int StatusCode { get; }
    public string ExampleName { get; }
    public string? Description { get; }

    public SwaggerResponseExampleAttribute(int statusCode, string exampleName, string? description = null)
    {
        StatusCode = statusCode;
        ExampleName = exampleName;
        Description = description;
    }
}

/// <summary>
/// Operation filter to add response examples based on SwaggerResponseExampleAttribute
/// </summary>
public class ResponseExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var responseExampleAttributes = context.MethodInfo
            .GetCustomAttributes<SwaggerResponseExampleAttribute>()
            .ToList();

        if (!responseExampleAttributes.Any())
            return;

        foreach (var attribute in responseExampleAttributes)
        {
            var statusCodeString = attribute.StatusCode.ToString();
            
            if (!operation.Responses.ContainsKey(statusCodeString))
                continue;

            var response = operation.Responses[statusCodeString];
            
            if (response.Content == null)
                continue;

            var example = GetExampleByName(attribute.ExampleName);
            if (example == null)
                continue;

            foreach (var mediaType in response.Content.Values)
            {
                mediaType.Example = example;
                
                if (!string.IsNullOrEmpty(attribute.Description))
                {
                    if (mediaType.Examples == null)
                        mediaType.Examples = new Dictionary<string, OpenApiExample>();
                    
                    mediaType.Examples[attribute.ExampleName] = new OpenApiExample
                    {
                        Summary = attribute.Description,
                        Value = example
                    };
                }
            }
        }
    }

    private static IOpenApiAny? GetExampleByName(string exampleName)
    {
        return exampleName switch
        {
            "CoffeeBeanSuccess" => CreateOpenApiObject(ResponseExamples.CoffeeBeanExample),
            "CoffeeBeansCollection" => CreateOpenApiObject(ResponseExamples.CoffeeBeansCollectionExample),
            "CoffeeBeansEmpty" => CreateOpenApiObject(ResponseExamples.EmptyCoffeeBeansCollection),
            "BrewSessionSuccess" => CreateOpenApiObject(ResponseExamples.BrewSessionExample),
            "BrewSessionsCollection" => CreateOpenApiObject(ResponseExamples.BrewSessionsCollectionExample),
            "BrewSessionsEmpty" => CreateOpenApiObject(new List<object>()),
            "BrewingEquipmentSuccess" => CreateOpenApiObject(ResponseExamples.BrewingEquipmentExample),
            "BrewingEquipmentCollection" => CreateOpenApiObject(ResponseExamples.BrewingEquipmentCollectionExample),
            "BrewingEquipmentEmpty" => CreateOpenApiObject(new List<object>()),
            "GrindSettingSuccess" => CreateOpenApiObject(ResponseExamples.GrindSettingExample),
            "GrindSettingsCollection" => CreateOpenApiObject(ResponseExamples.GrindSettingsCollectionExample),
            "GrindSettingsEmpty" => CreateOpenApiObject(new List<object>()),
            "HealthSuccess" => CreateOpenApiObject(ResponseExamples.HealthExample),
            "VendorsCollection" => CreateOpenApiObject(ResponseExamples.VendorsExample),
            "ModelsCollection" => CreateOpenApiObject(ResponseExamples.ModelsExample),
            "GrinderTypesCollection" => CreateOpenApiObject(ResponseExamples.GrinderTypesExample),
            "ValidationError" => CreateOpenApiObject(ResponseExamples.ValidationErrorExample),
            "EnumValidationError" => CreateOpenApiObject(ResponseExamples.EnumValidationErrorExample),
            "DetailedValidationError" => CreateOpenApiObject(ResponseExamples.DetailedValidationErrorExample),
            "NotFoundError" => CreateOpenApiObject(ResponseExamples.NotFoundErrorExample),
            "ConflictError" => CreateOpenApiObject(ResponseExamples.ConflictErrorExample),
            "InternalServerError" => CreateOpenApiObject(ResponseExamples.InternalServerErrorExample),
            _ => null
        };
    }

    private static IOpenApiAny CreateOpenApiObject(object obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        return new OpenApiString(json);
    }
}