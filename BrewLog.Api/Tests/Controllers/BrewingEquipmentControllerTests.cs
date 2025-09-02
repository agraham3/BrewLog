using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using BrewLog.Api.Controllers;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Tests.Controllers;

public class BrewingEquipmentControllerTests
{
    private readonly Mock<IBrewingEquipmentService> _mockService;
    private readonly Mock<ILogger<BrewingEquipmentController>> _mockLogger;
    private readonly BrewingEquipmentController _controller;

    public BrewingEquipmentControllerTests()
    {
        _mockService = new Mock<IBrewingEquipmentService>();
        _mockLogger = new Mock<ILogger<BrewingEquipmentController>>();
        _controller = new BrewingEquipmentController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetBrewingEquipment_ReturnsOkResult_WithListOfEquipment()
    {
        // Arrange
        var equipment = new List<BrewingEquipmentResponseDto>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine, Specifications = new Dictionary<string, string> { { "Pressure", "15 bar" } } },
            new() { Id = 2, Vendor = "Baratza", Model = "Encore", Type = EquipmentType.Grinder, Specifications = new Dictionary<string, string> { { "Burr Type", "Conical" } } }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<BrewingEquipmentFilterDto>()))
                   .ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetBrewingEquipment();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEquipment = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewingEquipmentResponseDto>>().Subject;
        returnedEquipment.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBrewingEquipment_WithTypeFilter_CallsServiceWithCorrectFilter()
    {
        // Arrange
        var equipment = new List<BrewingEquipmentResponseDto>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<BrewingEquipmentFilterDto>()))
                   .ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetBrewingEquipment(type: (int)EquipmentType.EspressoMachine, vendor: "Breville");

        // Assert
        _mockService.Verify(s => s.GetAllAsync(It.Is<BrewingEquipmentFilterDto>(f => 
            f.Type == EquipmentType.EspressoMachine && f.Vendor == "Breville")), Times.Once);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetBrewingEquipmentById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var equipment = new BrewingEquipmentResponseDto
        {
            Id = 1,
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string> { { "Pressure", "15 bar" } }
        };
        _mockService.Setup(s => s.GetByIdAsync(1))
                   .ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetBrewingEquipment(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEquipment = okResult.Value.Should().BeOfType<BrewingEquipmentResponseDto>().Subject;
        returnedEquipment.Id.Should().Be(1);
        returnedEquipment.Vendor.Should().Be("Breville");
        returnedEquipment.Model.Should().Be("Barista Express");
        returnedEquipment.Type.Should().Be(EquipmentType.EspressoMachine);
    }

    [Fact]
    public async Task GetBrewingEquipmentById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999))
                   .ReturnsAsync((BrewingEquipmentResponseDto?)null);

        // Act
        var result = await _controller.GetBrewingEquipment(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateBrewingEquipment_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string> { { "Pressure", "15 bar" } }
        };
        var createdEquipment = new BrewingEquipmentResponseDto
        {
            Id = 1,
            Vendor = createDto.Vendor,
            Model = createDto.Model,
            Type = createDto.Type,
            Specifications = createDto.Specifications
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
                   .ReturnsAsync(createdEquipment);

        // Act
        var result = await _controller.CreateBrewingEquipment(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedEquipment = createdResult.Value.Should().BeOfType<BrewingEquipmentResponseDto>().Subject;
        returnedEquipment.Vendor.Should().Be(createDto.Vendor);
        returnedEquipment.Model.Should().Be(createDto.Model);
        returnedEquipment.Type.Should().Be(createDto.Type);
        createdResult.ActionName.Should().Be(nameof(BrewingEquipmentController.GetBrewingEquipment));
    }

    [Fact]
    public async Task UpdateBrewingEquipment_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateBrewingEquipmentDto
        {
            Vendor = "Updated Vendor",
            Model = "Updated Model",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string> { { "Burr Type", "Flat" } }
        };
        var updatedEquipment = new BrewingEquipmentResponseDto
        {
            Id = 1,
            Vendor = updateDto.Vendor,
            Model = updateDto.Model,
            Type = updateDto.Type,
            Specifications = updateDto.Specifications
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
                   .ReturnsAsync(updatedEquipment);

        // Act
        var result = await _controller.UpdateBrewingEquipment(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEquipment = okResult.Value.Should().BeOfType<BrewingEquipmentResponseDto>().Subject;
        returnedEquipment.Vendor.Should().Be(updateDto.Vendor);
        returnedEquipment.Model.Should().Be(updateDto.Model);
        returnedEquipment.Type.Should().Be(updateDto.Type);
    }

    [Fact]
    public async Task UpdateBrewingEquipment_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateBrewingEquipmentDto
        {
            Vendor = "Updated Vendor",
            Model = "Updated Model",
            Type = EquipmentType.Grinder
        };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto))
                   .ThrowsAsync(new NotFoundException("Brewing equipment with ID 999 not found"));

        // Act
        var result = await _controller.UpdateBrewingEquipment(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteBrewingEquipment_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteBrewingEquipment(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteBrewingEquipment_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999))
                   .ThrowsAsync(new NotFoundException("Brewing equipment with ID 999 not found"));

        // Act
        var result = await _controller.DeleteBrewingEquipment(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteBrewingEquipment_WithEquipmentInUse_ReturnsConflict()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .ThrowsAsync(new InvalidOperationException("Cannot delete equipment that is referenced by brew sessions"));

        // Act
        var result = await _controller.DeleteBrewingEquipment(1);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task GetMostUsedBrewingEquipment_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var equipment = new List<BrewingEquipmentResponseDto>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine },
            new() { Id = 2, Vendor = "Baratza", Model = "Encore", Type = EquipmentType.Grinder }
        };
        _mockService.Setup(s => s.GetMostUsedAsync(5))
                   .ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetMostUsedBrewingEquipment(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEquipment = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewingEquipmentResponseDto>>().Subject;
        returnedEquipment.Should().HaveCount(2);
        _mockService.Verify(s => s.GetMostUsedAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetMostUsedBrewingEquipment_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetMostUsedBrewingEquipment(0);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetDistinctVendors_ReturnsOkResult_WithVendorList()
    {
        // Arrange
        var vendors = new List<string> { "Breville", "Baratza", "Hario" };
        _mockService.Setup(s => s.GetDistinctVendorsAsync())
                   .ReturnsAsync(vendors);

        // Act
        var result = await _controller.GetDistinctVendors();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedVendors = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
        returnedVendors.Should().HaveCount(3);
        returnedVendors.Should().Contain("Breville");
        returnedVendors.Should().Contain("Baratza");
        returnedVendors.Should().Contain("Hario");
    }

    [Fact]
    public async Task GetDistinctModels_ReturnsOkResult_WithModelList()
    {
        // Arrange
        var models = new List<string> { "Barista Express", "Encore", "V60" };
        _mockService.Setup(s => s.GetDistinctModelsAsync())
                   .ReturnsAsync(models);

        // Act
        var result = await _controller.GetDistinctModels();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedModels = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
        returnedModels.Should().HaveCount(3);
        returnedModels.Should().Contain("Barista Express");
        returnedModels.Should().Contain("Encore");
        returnedModels.Should().Contain("V60");
    }
}