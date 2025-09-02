using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;

namespace BrewLog.Api.Examples;

/// <summary>
/// Provides example response data for Swagger documentation
/// </summary>
public static class ResponseExamples
{
    // Coffee Bean Examples
    public static readonly CoffeeBeanResponseDto CoffeeBeanExample = new()
    {
        Id = 1,
        Name = "Ethiopian Yirgacheffe",
        Brand = "Blue Bottle Coffee",
        RoastLevel = RoastLevel.Medium,
        Origin = "Ethiopia, Yirgacheffe",
        CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
        ModifiedDate = new DateTime(2024, 1, 16, 14, 20, 0, DateTimeKind.Utc)
    };

    public static readonly List<CoffeeBeanResponseDto> CoffeeBeansCollectionExample = new()
    {
        CoffeeBeanExample,
        new()
        {
            Id = 2,
            Name = "Colombian Supremo",
            Brand = "Stumptown Coffee",
            RoastLevel = RoastLevel.MediumDark,
            Origin = "Colombia, Huila",
            CreatedDate = new DateTime(2024, 1, 10, 8, 15, 0, DateTimeKind.Utc),
            ModifiedDate = null
        }
    };

    public static readonly List<CoffeeBeanResponseDto> EmptyCoffeeBeansCollection = new();

    // Grind Setting Examples
    public static readonly GrindSettingResponseDto GrindSettingExample = new()
    {
        Id = 1,
        GrindSize = 15,
        GrindTime = TimeSpan.FromSeconds(20),
        GrindWeight = 22.5m,
        GrinderType = "Baratza Encore",
        Notes = "Medium grind for pour over, consistent particle size",
        CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };

    public static readonly List<GrindSettingResponseDto> GrindSettingsCollectionExample = new()
    {
        GrindSettingExample,
        new()
        {
            Id = 2,
            GrindSize = 8,
            GrindTime = TimeSpan.FromSeconds(15),
            GrindWeight = 18.0m,
            GrinderType = "Comandante C40",
            Notes = "Fine grind for espresso",
            CreatedDate = new DateTime(2024, 1, 12, 9, 45, 0, DateTimeKind.Utc)
        }
    };

    // Brewing Equipment Examples
    public static readonly BrewingEquipmentResponseDto BrewingEquipmentExample = new()
    {
        Id = 1,
        Vendor = "Hario",
        Model = "V60-02",
        Type = EquipmentType.PourOverSetup,
        Specifications = new Dictionary<string, string>
        {
            { "capacity", "1-4 cups" },
            { "material", "Ceramic" },
            { "dimensions", "11.6 x 10 x 8.2 cm" },
            { "weight", "340g" }
        },
        CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
    };

    public static readonly List<BrewingEquipmentResponseDto> BrewingEquipmentCollectionExample = new()
    {
        BrewingEquipmentExample,
        new()
        {
            Id = 2,
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "capacity", "2L water tank" },
                { "pressure", "15 bar" },
                { "power", "1600W" },
                { "dimensions", "31 x 31 x 40 cm" }
            },
            CreatedDate = new DateTime(2024, 1, 10, 8, 15, 0, DateTimeKind.Utc)
        }
    };

    // Brew Session Examples
    public static readonly BrewSessionResponseDto BrewSessionExample = new()
    {
        Id = 1,
        Method = BrewMethod.PourOver,
        WaterTemperature = 92.5m,
        BrewTime = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(30)),
        TastingNotes = "Bright acidity with notes of citrus and chocolate. Clean finish.",
        Rating = 8,
        IsFavorite = true,
        CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
        CoffeeBeanId = 1,
        GrindSettingId = 1,
        BrewingEquipmentId = 1,
        CoffeeBean = CoffeeBeanExample,
        GrindSetting = GrindSettingExample,
        BrewingEquipment = BrewingEquipmentExample
    };

    public static readonly List<BrewSessionResponseDto> BrewSessionsCollectionExample = new()
    {
        BrewSessionExample,
        new()
        {
            Id = 2,
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.0m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold with chocolate undertones",
            Rating = 9,
            IsFavorite = false,
            CreatedDate = new DateTime(2024, 1, 14, 7, 15, 0, DateTimeKind.Utc),
            CoffeeBeanId = 2,
            GrindSettingId = 2,
            BrewingEquipmentId = 2,
            CoffeeBean = CoffeeBeansCollectionExample[1],
            GrindSetting = GrindSettingsCollectionExample[1],
            BrewingEquipment = BrewingEquipmentCollectionExample[1]
        }
    };

    // Health Example
    public static readonly HealthResponseDto HealthExample = new()
    {
        Status = "Healthy",
        Timestamp = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
        Version = "1.0.0"
    };

    // Error Examples
    public static readonly object ValidationErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        title = "One or more validation errors occurred.",
        status = 400,
        errors = new
        {
            Name = new[] { "Coffee bean name is required" },
            RoastLevel = new[] { "Invalid RoastLevel. Accepted values: Light, MediumLight, Medium, MediumDark, Dark" },
            Method = new[] { "Invalid BrewMethod. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew" },
            WaterTemperature = new[] { "Espresso water temperature should be between 88°C and 96°C for optimal extraction" },
            Rating = new[] { "Rating must be between 1 and 10, where 1 is poor and 10 is excellent" },
            GrindSize = new[] { "Grind size must be between 1 (finest, for espresso) and 30 (coarsest, for cold brew)" },
            Type = new[] { "Invalid EquipmentType. Accepted values: EspressoMachine, Grinder, FrenchPress, PourOverSetup, DripMachine, AeroPress" }
        }
    };

    public static readonly object EnumValidationErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        title = "One or more validation errors occurred.",
        status = 400,
        errors = new
        {
            RoastLevel = new[] { "Invalid RoastLevel. Accepted values: Light, MediumLight, Medium, MediumDark, Dark" },
            Method = new[] { "Invalid BrewMethod. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew" }
        }
    };

    public static readonly object DetailedValidationErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        title = "One or more validation errors occurred.",
        status = 400,
        errors = new
        {
            Method = new[] { "Invalid BrewMethod. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew" },
            WaterTemperature = new[] { "Espresso water temperature should be between 88°C and 96°C for optimal extraction" },
            BrewTime = new[] { "Espresso brew time should be between 20 and 40 seconds for proper extraction" },
            Rating = new[] { "Rating must be between 1 and 10, where 1 is poor and 10 is excellent" },
            GrindSize = new[] { "Grind size must be between 1 (finest, for espresso) and 30 (coarsest, for cold brew)" },
            GrindWeight = new[] { "Grind weight must be between 0.1g and 1000g (typical range: 15-30g for most brewing methods)" }
        }
    };

    public static readonly object NotFoundErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        title = "Not Found",
        status = 404,
        detail = "Coffee bean with ID 999 not found"
    };

    public static readonly object ConflictErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        title = "Conflict",
        status = 409,
        detail = "Cannot delete coffee bean with ID 1 because it is referenced by existing brew sessions"
    };

    public static readonly object InternalServerErrorExample = new
    {
        type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        title = "Internal Server Error",
        status = 500,
        detail = "An unexpected error occurred while processing the request"
    };

    // Collection of distinct values examples
    public static readonly List<string> VendorsExample = new() { "Hario", "Breville", "Chemex", "Bodum" };
    public static readonly List<string> ModelsExample = new() { "V60-02", "Barista Express", "Classic Series", "Brazil" };
    public static readonly List<string> GrinderTypesExample = new() { "Baratza Encore", "Comandante C40", "Hario Mini Mill", "Breville Smart Grinder Pro" };
}

/// <summary>
/// Swagger schema filter to add response examples
/// </summary>
public class ResponseExamplesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CoffeeBeanResponseDto))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.CoffeeBeanExample);
        }
        else if (context.Type == typeof(IEnumerable<CoffeeBeanResponseDto>) || 
                 context.Type == typeof(List<CoffeeBeanResponseDto>))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.CoffeeBeansCollectionExample);
        }
        else if (context.Type == typeof(BrewSessionResponseDto))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.BrewSessionExample);
        }
        else if (context.Type == typeof(IEnumerable<BrewSessionResponseDto>) || 
                 context.Type == typeof(List<BrewSessionResponseDto>))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.BrewSessionsCollectionExample);
        }
        else if (context.Type == typeof(BrewingEquipmentResponseDto))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.BrewingEquipmentExample);
        }
        else if (context.Type == typeof(IEnumerable<BrewingEquipmentResponseDto>) || 
                 context.Type == typeof(List<BrewingEquipmentResponseDto>))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.BrewingEquipmentCollectionExample);
        }
        else if (context.Type == typeof(GrindSettingResponseDto))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.GrindSettingExample);
        }
        else if (context.Type == typeof(IEnumerable<GrindSettingResponseDto>) || 
                 context.Type == typeof(List<GrindSettingResponseDto>))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.GrindSettingsCollectionExample);
        }
        else if (context.Type == typeof(HealthResponseDto))
        {
            schema.Example = CreateOpenApiObject(ResponseExamples.HealthExample);
        }
        else if (context.Type == typeof(IEnumerable<string>) || context.Type == typeof(List<string>))
        {
            // Use vendors example for string collections (can be overridden in specific contexts)
            schema.Example = CreateOpenApiObject(ResponseExamples.VendorsExample);
        }
    }

    private static IOpenApiAny CreateOpenApiObject(object obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        return new OpenApiString(json);
    }
}