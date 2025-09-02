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

public class CoffeeBeansControllerTests
{
    private readonly Mock<ICoffeeBeanService> _mockService;
    private readonly Mock<ILogger<CoffeeBeansController>> _mockLogger;
    private readonly CoffeeBeansController _controller;

    public CoffeeBeansControllerTests()
    {
        _mockService = new Mock<ICoffeeBeanService>();
        _mockLogger = new Mock<ILogger<CoffeeBeansController>>();
        _controller = new CoffeeBeansController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetCoffeeBeans_ReturnsOkResult_WithListOfBeans()
    {
        // Arrange
        var beans = new List<CoffeeBeanResponseDto>
        {
            new() { Id = 1, Name = "Ethiopian Yirgacheffe", Brand = "Counter Culture", RoastLevel = RoastLevel.Light, Origin = "Ethiopia" },
            new() { Id = 2, Name = "Colombian Supremo", Brand = "Blue Bottle", RoastLevel = RoastLevel.Medium, Origin = "Colombia" }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CoffeeBeanFilterDto>()))
                   .ReturnsAsync(beans);

        // Act
        var result = await _controller.GetCoffeeBeans();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBeans = okResult.Value.Should().BeAssignableTo<IEnumerable<CoffeeBeanResponseDto>>().Subject;
        returnedBeans.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCoffeeBean_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var bean = new CoffeeBeanResponseDto
        {
            Id = 1,
            Name = "Ethiopian Yirgacheffe",
            Brand = "Counter Culture",
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };
        _mockService.Setup(s => s.GetByIdAsync(1))
                   .ReturnsAsync(bean);

        // Act
        var result = await _controller.GetCoffeeBean(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBean = okResult.Value.Should().BeOfType<CoffeeBeanResponseDto>().Subject;
        returnedBean.Id.Should().Be(1);
        returnedBean.Name.Should().Be("Ethiopian Yirgacheffe");
    }

    [Fact]
    public async Task GetCoffeeBean_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999))
                   .ReturnsAsync((CoffeeBeanResponseDto?)null);

        // Act
        var result = await _controller.GetCoffeeBean(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateCoffeeBean_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateCoffeeBeanDto
        {
            Name = "New Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };
        var createdBean = new CoffeeBeanResponseDto
        {
            Id = 1,
            Name = createDto.Name,
            Brand = createDto.Brand,
            RoastLevel = createDto.RoastLevel,
            Origin = createDto.Origin
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
                   .ReturnsAsync(createdBean);

        // Act
        var result = await _controller.CreateCoffeeBean(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedBean = createdResult.Value.Should().BeOfType<CoffeeBeanResponseDto>().Subject;
        returnedBean.Name.Should().Be(createDto.Name);
        createdResult.ActionName.Should().Be(nameof(CoffeeBeansController.GetCoffeeBean));
    }

    [Fact]
    public async Task UpdateCoffeeBean_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateCoffeeBeanDto
        {
            Name = "Updated Bean",
            Brand = "Updated Brand",
            RoastLevel = RoastLevel.Dark,
            Origin = "Updated Origin"
        };
        var updatedBean = new CoffeeBeanResponseDto
        {
            Id = 1,
            Name = updateDto.Name,
            Brand = updateDto.Brand,
            RoastLevel = updateDto.RoastLevel,
            Origin = updateDto.Origin
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
                   .ReturnsAsync(updatedBean);

        // Act
        var result = await _controller.UpdateCoffeeBean(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBean = okResult.Value.Should().BeOfType<CoffeeBeanResponseDto>().Subject;
        returnedBean.Name.Should().Be(updateDto.Name);
    }

    [Fact]
    public async Task UpdateCoffeeBean_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateCoffeeBeanDto
        {
            Name = "Updated Bean",
            Brand = "Updated Brand",
            RoastLevel = RoastLevel.Dark,
            Origin = "Updated Origin"
        };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto))
                   .ThrowsAsync(new NotFoundException("CoffeeBean", 999));

        // Act
        var result = await _controller.UpdateCoffeeBean(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteCoffeeBean_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCoffeeBean(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteCoffeeBean_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999))
                   .ThrowsAsync(new NotFoundException("CoffeeBean", 999));

        // Act
        var result = await _controller.DeleteCoffeeBean(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteCoffeeBean_WithActiveBrewSessions_ReturnsConflict()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .ThrowsAsync(new InvalidOperationException("Cannot delete coffee bean with active brew sessions"));

        // Act
        var result = await _controller.DeleteCoffeeBean(1);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task GetRecentCoffeeBeans_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var beans = new List<CoffeeBeanResponseDto>
        {
            new() { Id = 1, Name = "Recent Bean 1", Brand = "Brand 1", RoastLevel = RoastLevel.Light, Origin = "Origin 1" },
            new() { Id = 2, Name = "Recent Bean 2", Brand = "Brand 2", RoastLevel = RoastLevel.Medium, Origin = "Origin 2" }
        };
        _mockService.Setup(s => s.GetRecentlyAddedAsync(5))
                   .ReturnsAsync(beans);

        // Act
        var result = await _controller.GetRecentCoffeeBeans(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBeans = okResult.Value.Should().BeAssignableTo<IEnumerable<CoffeeBeanResponseDto>>().Subject;
        returnedBeans.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentCoffeeBeans_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetRecentCoffeeBeans(0);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetMostUsedCoffeeBeans_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var beans = new List<CoffeeBeanResponseDto>
        {
            new() { Id = 1, Name = "Popular Bean 1", Brand = "Brand 1", RoastLevel = RoastLevel.Light, Origin = "Origin 1" },
            new() { Id = 2, Name = "Popular Bean 2", Brand = "Brand 2", RoastLevel = RoastLevel.Medium, Origin = "Origin 2" }
        };
        _mockService.Setup(s => s.GetMostUsedAsync(5))
                   .ReturnsAsync(beans);

        // Act
        var result = await _controller.GetMostUsedCoffeeBeans(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBeans = okResult.Value.Should().BeAssignableTo<IEnumerable<CoffeeBeanResponseDto>>().Subject;
        returnedBeans.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMostUsedCoffeeBeans_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetMostUsedCoffeeBeans(101);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}