using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BrewLog.Api.Data;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BrewLog.Api.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BrewLogDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<BrewLogDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
        });
    }
}

/// <summary>
/// Integration tests to validate enum serialization behavior in API responses and requests
/// </summary>
public class EnumSerializationIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EnumSerializationIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GET_CoffeeBeans_Should_Return_Enums_As_Strings()
    {
        // Arrange - First create a coffee bean with enum values
        var createDto = new CreateCoffeeBeanDto
        {
            Name = "Test Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };

        // Act - Create the coffee bean
        var createResponse = await _client.PostAsJsonAsync("/api/coffeebeans", createDto);
        createResponse.EnsureSuccessStatusCode();

        // Get all coffee beans
        var getResponse = await _client.GetAsync("/api/coffeebeans");
        getResponse.EnsureSuccessStatusCode();

        var responseContent = await getResponse.Content.ReadAsStringAsync();
        var coffeeBeans = JsonSerializer.Deserialize<List<CoffeeBeanResponseDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        coffeeBeans.Should().NotBeEmpty();
        var testBean = coffeeBeans!.FirstOrDefault(b => b.Name == "Test Bean");
        testBean.Should().NotBeNull();

        // Verify that the JSON contains string enum values, not integers
        responseContent.Should().Contain("\"Medium\"");
        responseContent.Should().NotContain("\"roastLevel\":2"); // Should not contain integer value
    }

    [Fact]
    public async Task POST_CoffeeBean_Should_Accept_String_Enum_Values()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "String Enum Test Bean",
            "brand": "Test Brand",
            "roastLevel": "Dark",
            "origin": "Test Origin"
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/coffeebeans", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdBean = JsonSerializer.Deserialize<CoffeeBeanResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdBean.Should().NotBeNull();
        createdBean!.RoastLevel.Should().Be(RoastLevel.Dark);

        // Verify response contains string enum value
        responseContent.Should().Contain("\"Dark\"");
    }

    [Fact]
    public async Task POST_CoffeeBean_Should_Accept_Integer_Enum_Values_For_Backward_Compatibility()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "Integer Enum Test Bean",
            "brand": "Test Brand",
            "roastLevel": 1,
            "origin": "Test Origin"
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/coffeebeans", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdBean = JsonSerializer.Deserialize<CoffeeBeanResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdBean.Should().NotBeNull();
        createdBean!.RoastLevel.Should().Be(RoastLevel.MediumLight); // 1 = MediumLight

        // Verify response still returns string enum value
        responseContent.Should().Contain("\"MediumLight\"");
    }

    [Fact]
    public async Task POST_CoffeeBean_Should_Accept_Case_Insensitive_String_Enum_Values()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "Case Insensitive Test Bean",
            "brand": "Test Brand",
            "roastLevel": "medium",
            "origin": "Test Origin"
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/coffeebeans", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdBean = JsonSerializer.Deserialize<CoffeeBeanResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdBean.Should().NotBeNull();
        createdBean!.RoastLevel.Should().Be(RoastLevel.Medium);

        // Verify response returns proper case string enum value
        responseContent.Should().Contain("\"Medium\"");
    }

    [Fact]
    public async Task POST_CoffeeBean_Should_Return_Validation_Error_For_Invalid_String_Enum()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "Invalid Enum Test Bean",
            "brand": "Test Brand",
            "roastLevel": "SuperDark",
            "origin": "Test Origin"
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/coffeebeans", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("SuperDark");
        responseContent.Should().Contain("RoastLevel");
        responseContent.Should().Contain("Valid values are:");
    }

    [Fact]
    public async Task POST_CoffeeBean_Should_Return_Validation_Error_For_Invalid_Integer_Enum()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "Invalid Integer Enum Test Bean",
            "brand": "Test Brand",
            "roastLevel": 99,
            "origin": "Test Origin"
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/coffeebeans", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("99");
        responseContent.Should().Contain("RoastLevel");
    }

    [Fact]
    public async Task POST_BrewSession_Should_Handle_BrewMethod_Enum_Serialization()
    {
        // Arrange - First create a coffee bean
        var coffeeBeanDto = new CreateCoffeeBeanDto
        {
            Name = "Test Bean for Brew Session",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };

        var coffeeBeanResponse = await _client.PostAsJsonAsync("/api/coffeebeans", coffeeBeanDto);
        coffeeBeanResponse.EnsureSuccessStatusCode();
        var coffeeBeanContent = await coffeeBeanResponse.Content.ReadAsStringAsync();
        var createdCoffeeBean = JsonSerializer.Deserialize<CoffeeBeanResponseDto>(coffeeBeanContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // First create a grind setting
        var grindSettingDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(20),
            GrindWeight = 22.5m,
            GrinderType = "Test Grinder",
            Notes = "Test grind setting"
        };

        var grindSettingResponse = await _client.PostAsJsonAsync("/api/grindsettings", grindSettingDto);
        var grindSettingContent = await grindSettingResponse.Content.ReadAsStringAsync();
        var createdGrindSetting = JsonSerializer.Deserialize<GrindSettingResponseDto>(grindSettingContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Create brew session with string enum value
        var jsonContent = $@"{{
            ""coffeeBeanId"": {createdCoffeeBean!.Id},
            ""grindSettingId"": {createdGrindSetting!.Id},
            ""method"": ""PourOver"",
            ""waterTemperature"": 92.5,
            ""brewTime"": ""00:04:30"",
            ""rating"": 4,
            ""tastingNotes"": ""Great pour over session""
        }}";

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/brewsessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdSession = JsonSerializer.Deserialize<BrewSessionResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdSession.Should().NotBeNull();
        createdSession!.Method.Should().Be(BrewMethod.PourOver);

        // Verify response contains string enum value
        responseContent.Should().Contain("\"PourOver\"");
    }

    [Fact]
    public async Task POST_BrewingEquipment_Should_Handle_EquipmentType_Enum_Serialization()
    {
        // Arrange
        var jsonContent = """
        {
            "name": "Test Grinder",
            "type": "Grinder",
            "vendor": "Test Vendor",
            "model": "Test Model",
            "specifications": {
                "burr_type": "ceramic",
                "grind_settings": "40"
            }
        }
        """;

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/brewingequipment", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdEquipment = JsonSerializer.Deserialize<BrewingEquipmentResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdEquipment.Should().NotBeNull();
        createdEquipment!.Type.Should().Be(EquipmentType.Grinder);

        // Verify response contains string enum value
        responseContent.Should().Contain("\"Grinder\"");
    }

    [Fact]
    public async Task GET_CoffeeBeans_With_RoastLevel_Filter_Should_Accept_String_Enum_Parameter()
    {
        // Arrange - Create coffee beans with different roast levels
        var lightRoastBean = new CreateCoffeeBeanDto
        {
            Name = "Light Roast Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Light,
            Origin = "Test Origin"
        };

        var darkRoastBean = new CreateCoffeeBeanDto
        {
            Name = "Dark Roast Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Dark,
            Origin = "Test Origin"
        };

        await _client.PostAsJsonAsync("/api/coffeebeans", lightRoastBean);
        await _client.PostAsJsonAsync("/api/coffeebeans", darkRoastBean);

        // Act - Filter by roast level using string parameter
        var response = await _client.GetAsync("/api/coffeebeans?roastLevel=Light");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var coffeeBeans = JsonSerializer.Deserialize<List<CoffeeBeanResponseDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        coffeeBeans.Should().NotBeEmpty();
        coffeeBeans!.Should().OnlyContain(b => b.RoastLevel == RoastLevel.Light);

        // Verify response contains string enum values
        responseContent.Should().Contain("\"Light\"");
        responseContent.Should().NotContain("\"Dark\"");
    }

    [Fact]
    public async Task GET_BrewSessions_With_Method_Filter_Should_Accept_String_Enum_Parameter()
    {
        // Arrange - First create a coffee bean
        var coffeeBeanDto = new CreateCoffeeBeanDto
        {
            Name = "Filter Test Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };

        var coffeeBeanResponse = await _client.PostAsJsonAsync("/api/coffeebeans", coffeeBeanDto);
        var coffeeBeanContent = await coffeeBeanResponse.Content.ReadAsStringAsync();
        var createdCoffeeBean = JsonSerializer.Deserialize<CoffeeBeanResponseDto>(coffeeBeanContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // First create a grind setting
        var grindSettingDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(20),
            GrindWeight = 22.5m,
            GrinderType = "Test Grinder",
            Notes = "Test grind setting"
        };

        var grindSettingResponse = await _client.PostAsJsonAsync("/api/grindsettings", grindSettingDto);
        var grindSettingContent = await grindSettingResponse.Content.ReadAsStringAsync();
        var createdGrindSetting = JsonSerializer.Deserialize<GrindSettingResponseDto>(grindSettingContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Create brew sessions with different methods
        var espressoSession = new CreateBrewSessionDto
        {
            CoffeeBeanId = createdCoffeeBean!.Id,
            GrindSettingId = createdGrindSetting!.Id,
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.0m,
            BrewTime = TimeSpan.FromSeconds(30),
            Rating = 4,
            TastingNotes = "Espresso session"
        };

        var pourOverSession = new CreateBrewSessionDto
        {
            CoffeeBeanId = createdCoffeeBean.Id,
            GrindSettingId = createdGrindSetting.Id,
            Method = BrewMethod.PourOver,
            WaterTemperature = 92.0m,
            BrewTime = TimeSpan.FromMinutes(4),
            Rating = 5,
            TastingNotes = "Pour over session"
        };

        await _client.PostAsJsonAsync("/api/brewsessions", espressoSession);
        await _client.PostAsJsonAsync("/api/brewsessions", pourOverSession);

        // Act - Filter by method using string parameter
        var response = await _client.GetAsync("/api/brewsessions?method=Espresso");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var brewSessions = JsonSerializer.Deserialize<List<BrewSessionResponseDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        brewSessions.Should().NotBeEmpty();
        brewSessions!.Should().OnlyContain(s => s.Method == BrewMethod.Espresso);

        // Verify response contains string enum values
        responseContent.Should().Contain("\"Espresso\"");
        responseContent.Should().NotContain("\"PourOver\"");
    }
}