using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services;
using BrewLog.Api.Services.Exceptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace BrewLog.Api.Tests.Services;

public class BrewingEquipmentServiceTests
{
    private readonly Mock<IBrewingEquipmentRepository> _mockRepository;
    private readonly Mock<IBrewSessionRepository> _mockBrewSessionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateBrewingEquipmentDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateBrewingEquipmentDto>> _mockUpdateValidator;
    private readonly BrewingEquipmentService _service;

    public BrewingEquipmentServiceTests()
    {
        _mockRepository = new Mock<IBrewingEquipmentRepository>();
        _mockBrewSessionRepository = new Mock<IBrewSessionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateBrewingEquipmentDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateBrewingEquipmentDto>>();

        _service = new BrewingEquipmentService(
            _mockRepository.Object,
            _mockBrewSessionRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidEspressoMachine_ReturnsCreatedEquipment()
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "BarPressure", "15" },
                { "BoilerCapacity", "1.8" }
            }
        };
        var equipment = new BrewingEquipment
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = createDto.Specifications
        };
        var createdEquipment = new BrewingEquipment
        {
            Id = 1,
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = createDto.Specifications,
            CreatedDate = DateTime.UtcNow
        };
        var expectedDto = new BrewingEquipmentResponseDto
        {
            Id = 1,
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = createDto.Specifications
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync(null, "Breville", "Barista Express", null)).ReturnsAsync(new List<BrewingEquipment>());
        _mockMapper.Setup(m => m.Map<BrewingEquipment>(createDto)).Returns(equipment);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<BrewingEquipment>())).ReturnsAsync(createdEquipment);
        _mockMapper.Setup(m => m.Map<BrewingEquipmentResponseDto>(createdEquipment)).Returns(expectedDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<BrewingEquipment>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidEspressoMachineBarPressure_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "BarPressure", "25" } // Invalid: too high
            }
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync(null, "Breville", "Barista Express", null)).ReturnsAsync(new List<BrewingEquipment>());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Bar pressure must be a positive number between 0 and 20.");
    }

    [Fact]
    public async Task CreateAsync_InvalidGrinderMotorPower_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "Baratza",
            Model = "Encore",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string>
            {
                { "MotorPower", "-100" } // Invalid: negative
            }
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync(null, "Baratza", "Encore", null)).ReturnsAsync(new List<BrewingEquipment>());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Motor power must be a positive number.");
    }

    [Fact]
    public async Task CreateAsync_DuplicateVendorAndModel_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>()
        };
        var existingEquipment = new List<BrewingEquipment>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine }
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync(null, "Breville", "Barista Express", null)).ReturnsAsync(existingEquipment);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Equipment with vendor 'Breville' and model 'Barista Express' already exists.");
    }

    [Fact]
    public async Task DeleteAsync_ExistingIdWithReferences_ThrowsReferentialIntegrityException()
    {
        // Arrange
        var existingEquipment = new BrewingEquipment
        {
            Id = 1,
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine
        };
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, BrewingEquipmentId = 1 }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEquipment);
        _mockBrewSessionRepository.Setup(r => r.GetByEquipmentIdAsync(1)).ReturnsAsync(brewSessions);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(1))
            .Should().ThrowAsync<ReferentialIntegrityException>()
            .WithMessage("Cannot delete equipment 'Breville Barista Express' because it is referenced by 1 brew session(s). Please delete the associated brew sessions first.");
    }

    [Theory]
    [InlineData(EquipmentType.FrenchPress, "Capacity", "-1", "Capacity must be a positive number.")]
    [InlineData(EquipmentType.FrenchPress, "Material", "", "Material cannot be empty.")]
    [InlineData(EquipmentType.DripMachine, "BrewTemperature", "120", "Brew temperature must be between 80 and 100 degrees Celsius.")]
    [InlineData(EquipmentType.DripMachine, "BrewTemperature", "70", "Brew temperature must be between 80 and 100 degrees Celsius.")]
    public async Task CreateAsync_InvalidSpecifications_ThrowsBusinessValidationException(
        EquipmentType type, string specKey, string specValue, string expectedMessage)
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "TestVendor",
            Model = "TestModel",
            Type = type,
            Specifications = new Dictionary<string, string>
            {
                { specKey, specValue }
            }
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync(null, "TestVendor", "TestModel", null)).ReturnsAsync(new List<BrewingEquipment>());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GetDistinctVendorsAsync_ReturnsDistinctVendors()
    {
        // Arrange
        var vendors = new List<string> { "Breville", "Baratza", "Hario" };
        _mockRepository.Setup(r => r.GetDistinctVendorsAsync()).ReturnsAsync(vendors);

        // Act
        var result = await _service.GetDistinctVendorsAsync();

        // Assert
        result.Should().BeEquivalentTo(vendors);
    }

    [Fact]
    public async Task GetDistinctModelsAsync_ReturnsDistinctModels()
    {
        // Arrange
        var models = new List<string> { "Barista Express", "Encore", "V60" };
        _mockRepository.Setup(r => r.GetDistinctModelsAsync()).ReturnsAsync(models);

        // Act
        var result = await _service.GetDistinctModelsAsync();

        // Assert
        result.Should().BeEquivalentTo(models);
    }
}