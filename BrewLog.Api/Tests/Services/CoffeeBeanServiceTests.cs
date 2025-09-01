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

public class CoffeeBeanServiceTests
{
    private readonly Mock<ICoffeeBeanRepository> _mockRepository;
    private readonly Mock<IBrewSessionRepository> _mockBrewSessionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateCoffeeBeanDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateCoffeeBeanDto>> _mockUpdateValidator;
    private readonly CoffeeBeanService _service;

    public CoffeeBeanServiceTests()
    {
        _mockRepository = new Mock<ICoffeeBeanRepository>();
        _mockBrewSessionRepository = new Mock<IBrewSessionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateCoffeeBeanDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateCoffeeBeanDto>>();

        _service = new CoffeeBeanService(
            _mockRepository.Object,
            _mockBrewSessionRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object);
    }

    [Fact]
    public async Task GetAllAsync_WithoutFilter_ReturnsAllBeans()
    {
        // Arrange
        var beans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" },
            new() { Id = 2, Name = "Bean2", Brand = "Brand2", RoastLevel = RoastLevel.Dark, Origin = "Origin2" }
        };
        var expectedDtos = new List<CoffeeBeanResponseDto>
        {
            new() { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" },
            new() { Id = 2, Name = "Bean2", Brand = "Brand2", RoastLevel = RoastLevel.Dark, Origin = "Origin2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(beans);
        _mockMapper.Setup(m => m.Map<IEnumerable<CoffeeBeanResponseDto>>(beans)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithFilter_ReturnsFilteredBeans()
    {
        // Arrange
        var filter = new CoffeeBeanFilterDto { Brand = "TestBrand", RoastLevel = RoastLevel.Medium };
        var beans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "Bean1", Brand = "TestBrand", RoastLevel = RoastLevel.Medium, Origin = "Origin1" }
        };
        var expectedDtos = new List<CoffeeBeanResponseDto>
        {
            new() { Id = 1, Name = "Bean1", Brand = "TestBrand", RoastLevel = RoastLevel.Medium, Origin = "Origin1" }
        };

        _mockRepository.Setup(r => r.GetFilteredAsync("TestBrand", RoastLevel.Medium, null, null)).ReturnsAsync(beans);
        _mockMapper.Setup(m => m.Map<IEnumerable<CoffeeBeanResponseDto>>(beans)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllAsync(filter);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        _mockRepository.Verify(r => r.GetFilteredAsync("TestBrand", RoastLevel.Medium, null, null), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsBean()
    {
        // Arrange
        var bean = new CoffeeBean { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" };
        var expectedDto = new CoffeeBeanResponseDto { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bean);
        _mockMapper.Setup(m => m.Map<CoffeeBeanResponseDto>(bean)).Returns(expectedDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CoffeeBean?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedBean()
    {
        // Arrange
        var createDto = new CreateCoffeeBeanDto { Name = "NewBean", Brand = "NewBrand", RoastLevel = RoastLevel.Medium, Origin = "NewOrigin" };
        var bean = new CoffeeBean { Name = "NewBean", Brand = "NewBrand", RoastLevel = RoastLevel.Medium, Origin = "NewOrigin" };
        var createdBean = new CoffeeBean { Id = 1, Name = "NewBean", Brand = "NewBrand", RoastLevel = RoastLevel.Medium, Origin = "NewOrigin", CreatedDate = DateTime.UtcNow };
        var expectedDto = new CoffeeBeanResponseDto { Id = 1, Name = "NewBean", Brand = "NewBrand", RoastLevel = RoastLevel.Medium, Origin = "NewOrigin" };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync("NewBrand", null, null, "NewBean")).ReturnsAsync(new List<CoffeeBean>());
        _mockMapper.Setup(m => m.Map<CoffeeBean>(createDto)).Returns(bean);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<CoffeeBean>())).ReturnsAsync(createdBean);
        _mockMapper.Setup(m => m.Map<CoffeeBeanResponseDto>(createdBean)).Returns(expectedDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<CoffeeBean>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidDto_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateCoffeeBeanDto { Name = "", Brand = "Brand", RoastLevel = RoastLevel.Medium, Origin = "Origin" };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new("Name", "Name is required")
        });

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(validationResult);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Validation failed: Name is required");
    }

    [Fact]
    public async Task CreateAsync_DuplicateNameAndBrand_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateCoffeeBeanDto { Name = "ExistingBean", Brand = "ExistingBrand", RoastLevel = RoastLevel.Medium, Origin = "Origin" };
        var existingBeans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "ExistingBean", Brand = "ExistingBrand", RoastLevel = RoastLevel.Medium, Origin = "Origin" }
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetFilteredAsync("ExistingBrand", null, null, "ExistingBean")).ReturnsAsync(existingBeans);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("A coffee bean with name 'ExistingBean' from brand 'ExistingBrand' already exists.");
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ReturnsUpdatedBean()
    {
        // Arrange
        var updateDto = new UpdateCoffeeBeanDto { Name = "UpdatedBean", Brand = "UpdatedBrand", RoastLevel = RoastLevel.Dark, Origin = "UpdatedOrigin" };
        var existingBean = new CoffeeBean { Id = 1, Name = "OldBean", Brand = "OldBrand", RoastLevel = RoastLevel.Medium, Origin = "OldOrigin", CreatedDate = DateTime.UtcNow.AddDays(-1) };
        var updatedBean = new CoffeeBean { Id = 1, Name = "UpdatedBean", Brand = "UpdatedBrand", RoastLevel = RoastLevel.Dark, Origin = "UpdatedOrigin", CreatedDate = DateTime.UtcNow.AddDays(-1), ModifiedDate = DateTime.UtcNow };
        var expectedDto = new CoffeeBeanResponseDto { Id = 1, Name = "UpdatedBean", Brand = "UpdatedBrand", RoastLevel = RoastLevel.Dark, Origin = "UpdatedOrigin" };

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingBean);
        _mockRepository.Setup(r => r.GetFilteredAsync("UpdatedBrand", null, null, "UpdatedBean")).ReturnsAsync(new List<CoffeeBean>());
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<CoffeeBean>())).ReturnsAsync(updatedBean);
        _mockMapper.Setup(m => m.Map<CoffeeBeanResponseDto>(updatedBean)).Returns(expectedDto);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        existingBean.Name.Should().Be("UpdatedBean");
        existingBean.Brand.Should().Be("UpdatedBrand");
        existingBean.RoastLevel.Should().Be(RoastLevel.Dark);
        existingBean.Origin.Should().Be("UpdatedOrigin");
        existingBean.ModifiedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateCoffeeBeanDto { Name = "UpdatedBean", Brand = "UpdatedBrand", RoastLevel = RoastLevel.Dark, Origin = "UpdatedOrigin" };

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CoffeeBean?)null);

        // Act & Assert
        await _service.Invoking(s => s.UpdateAsync(999, updateDto))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("CoffeeBean with ID 999 was not found.");
    }

    [Fact]
    public async Task DeleteAsync_ExistingIdWithoutReferences_DeletesBean()
    {
        // Arrange
        var existingBean = new CoffeeBean { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingBean);
        _mockBrewSessionRepository.Setup(r => r.GetByCoffeeBeanIdAsync(1)).ReturnsAsync(new List<BrewSession>());

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingIdWithReferences_ThrowsReferentialIntegrityException()
    {
        // Arrange
        var existingBean = new CoffeeBean { Id = 1, Name = "Bean1", Brand = "Brand1", RoastLevel = RoastLevel.Medium, Origin = "Origin1" };
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, CoffeeBeanId = 1 },
            new() { Id = 2, CoffeeBeanId = 1 }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingBean);
        _mockBrewSessionRepository.Setup(r => r.GetByCoffeeBeanIdAsync(1)).ReturnsAsync(brewSessions);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(1))
            .Should().ThrowAsync<ReferentialIntegrityException>()
            .WithMessage("Cannot delete coffee bean 'Bean1' because it is referenced by 2 brew session(s). Please delete the associated brew sessions first.");
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CoffeeBean?)null);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(999))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("CoffeeBean with ID 999 was not found.");
    }

    [Fact]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _service.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}