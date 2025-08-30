using AutoMapper;
using BrewLog.Api.DTOs;
using Xunit;

namespace BrewLog.Api.Tests.DTOs;

public class AutoMapperConfigurationTests
{
    [Fact]
    public void AutoMapperConfiguration_ShouldBeValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CoffeeBeanMappingProfile>();
            cfg.AddProfile<GrindSettingMappingProfile>();
            cfg.AddProfile<BrewingEquipmentMappingProfile>();
            cfg.AddProfile<BrewSessionMappingProfile>();
            cfg.AddProfile<AnalyticsMappingProfile>();
        });

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void AutoMapperConfiguration_ShouldCreateMapperSuccessfully()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CoffeeBeanMappingProfile>();
            cfg.AddProfile<GrindSettingMappingProfile>();
            cfg.AddProfile<BrewingEquipmentMappingProfile>();
            cfg.AddProfile<BrewSessionMappingProfile>();
            cfg.AddProfile<AnalyticsMappingProfile>();
        });

        // Act
        var mapper = configuration.CreateMapper();

        // Assert
        Assert.NotNull(mapper);
    }
}