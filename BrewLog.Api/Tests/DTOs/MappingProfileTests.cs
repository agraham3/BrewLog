using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using FluentAssertions;
using Xunit;

namespace BrewLog.Api.Tests.DTOs;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CoffeeBeanMappingProfile>();
            cfg.AddProfile<GrindSettingMappingProfile>();
            cfg.AddProfile<BrewingEquipmentMappingProfile>();
            cfg.AddProfile<BrewSessionMappingProfile>();
            cfg.AddProfile<AnalyticsMappingProfile>();
        });

        configuration.AssertConfigurationIsValid();
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void CoffeeBeanMappingProfile_ShouldMapEntityToResponseDto()
    {
        // Arrange
        var coffeeBean = new CoffeeBean
        {
            Id = 1,
            Name = "Test Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = _mapper.Map<CoffeeBeanResponseDto>(coffeeBean);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(coffeeBean.Id);
        result.Name.Should().Be(coffeeBean.Name);
        result.Brand.Should().Be(coffeeBean.Brand);
        result.RoastLevel.Should().Be(coffeeBean.RoastLevel);
        result.Origin.Should().Be(coffeeBean.Origin);
        result.CreatedDate.Should().Be(coffeeBean.CreatedDate);
        result.ModifiedDate.Should().Be(coffeeBean.ModifiedDate);
    }

    [Fact]
    public void CoffeeBeanMappingProfile_ShouldMapCreateDtoToEntity()
    {
        // Arrange
        var createDto = new CreateCoffeeBeanDto
        {
            Name = "Test Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };

        // Act
        var result = _mapper.Map<CoffeeBean>(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0); // Should be ignored
        result.Name.Should().Be(createDto.Name);
        result.Brand.Should().Be(createDto.Brand);
        result.RoastLevel.Should().Be(createDto.RoastLevel);
        result.Origin.Should().Be(createDto.Origin);
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1)); // Should be set to current time
        result.ModifiedDate.Should().BeNull(); // Should be ignored
    }

    [Fact]
    public void GrindSettingMappingProfile_ShouldMapEntityToResponseDto()
    {
        // Arrange
        var grindSetting = new GrindSetting
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes",
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = _mapper.Map<GrindSettingResponseDto>(grindSetting);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(grindSetting.Id);
        result.GrindSize.Should().Be(grindSetting.GrindSize);
        result.GrindTime.Should().Be(grindSetting.GrindTime);
        result.GrindWeight.Should().Be(grindSetting.GrindWeight);
        result.GrinderType.Should().Be(grindSetting.GrinderType);
        result.Notes.Should().Be(grindSetting.Notes);
        result.CreatedDate.Should().Be(grindSetting.CreatedDate);
    }

    [Fact]
    public void BrewingEquipmentMappingProfile_ShouldMapEntityToResponseDto()
    {
        // Arrange
        var equipment = new BrewingEquipment
        {
            Id = 1,
            Vendor = "Test Vendor",
            Model = "Test Model",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string> { { "Pressure", "9 bar" } },
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = _mapper.Map<BrewingEquipmentResponseDto>(equipment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(equipment.Id);
        result.Vendor.Should().Be(equipment.Vendor);
        result.Model.Should().Be(equipment.Model);
        result.Type.Should().Be(equipment.Type);
        result.Specifications.Should().BeEquivalentTo(equipment.Specifications);
        result.CreatedDate.Should().Be(equipment.CreatedDate);
    }

    [Fact]
    public void BrewSessionMappingProfile_ShouldMapEntityToResponseDto()
    {
        // Arrange
        var brewSession = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.5m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Great taste",
            Rating = 8,
            IsFavorite = true,
            CreatedDate = DateTime.UtcNow,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1,
            CoffeeBean = new CoffeeBean { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
            GrindSetting = new GrindSetting { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(30), GrindWeight = 20.5m, GrinderType = "Burr" },
            BrewingEquipment = new BrewingEquipment { Id = 1, Vendor = "Test Vendor", Model = "Test Model", Type = EquipmentType.EspressoMachine }
        };

        // Act
        var result = _mapper.Map<BrewSessionResponseDto>(brewSession);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(brewSession.Id);
        result.Method.Should().Be(brewSession.Method);
        result.WaterTemperature.Should().Be(brewSession.WaterTemperature);
        result.BrewTime.Should().Be(brewSession.BrewTime);
        result.TastingNotes.Should().Be(brewSession.TastingNotes);
        result.Rating.Should().Be(brewSession.Rating);
        result.IsFavorite.Should().Be(brewSession.IsFavorite);
        result.CreatedDate.Should().Be(brewSession.CreatedDate);
        result.CoffeeBeanId.Should().Be(brewSession.CoffeeBeanId);
        result.GrindSettingId.Should().Be(brewSession.GrindSettingId);
        result.BrewingEquipmentId.Should().Be(brewSession.BrewingEquipmentId);
        result.CoffeeBean.Should().NotBeNull();
        result.GrindSetting.Should().NotBeNull();
        result.BrewingEquipment.Should().NotBeNull();
    }

    [Fact]
    public void AnalyticsMappingProfile_ShouldMapBrewSessionToRecentBrewSessionDto()
    {
        // Arrange
        var brewSession = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            Rating = 8,
            IsFavorite = true,
            CreatedDate = DateTime.UtcNow,
            CoffeeBean = new CoffeeBean { Name = "Test Bean" }
        };

        // Act
        var result = _mapper.Map<RecentBrewSessionDto>(brewSession);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(brewSession.Id);
        result.Method.Should().Be(brewSession.Method);
        result.Rating.Should().Be(brewSession.Rating);
        result.IsFavorite.Should().Be(brewSession.IsFavorite);
        result.CreatedDate.Should().Be(brewSession.CreatedDate);
        result.CoffeeBeanName.Should().Be(brewSession.CoffeeBean.Name);
    }

    [Fact]
    public void UpdateCoffeeBeanDto_ShouldSetModifiedDate()
    {
        // Arrange
        var updateDto = new UpdateCoffeeBeanDto
        {
            Name = "Updated Bean",
            Brand = "Updated Brand",
            RoastLevel = RoastLevel.Dark,
            Origin = "Updated Origin"
        };

        // Act
        var result = _mapper.Map<CoffeeBean>(updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updateDto.Name);
        result.Brand.Should().Be(updateDto.Brand);
        result.RoastLevel.Should().Be(updateDto.RoastLevel);
        result.Origin.Should().Be(updateDto.Origin);
        result.ModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}