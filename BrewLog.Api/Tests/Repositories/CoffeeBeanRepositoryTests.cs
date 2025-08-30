using Xunit;
using FluentAssertions;
using BrewLog.Api.Repositories;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Repositories;

public class CoffeeBeanRepositoryTests : RepositoryTestBase
{
    private readonly CoffeeBeanRepository _repository;

    public CoffeeBeanRepositoryTests()
    {
        _repository = new CoffeeBeanRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectBean()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Ethiopian Yirgacheffe");
        result.Brand.Should().Be("Blue Bottle");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBeans()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(b => b.Name == "Ethiopian Yirgacheffe");
        result.Should().Contain(b => b.Name == "Colombian Supremo");
        result.Should().Contain(b => b.Name == "French Roast");
    }

    [Fact]
    public async Task GetByBrandAsync_ExistingBrand_ReturnsMatchingBeans()
    {
        // Act
        var result = await _repository.GetByBrandAsync("Blue Bottle");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(b => b.Brand.Should().Be("Blue Bottle"));
    }

    [Fact]
    public async Task GetByBrandAsync_PartialMatch_ReturnsMatchingBeans()
    {
        // Act
        var result = await _repository.GetByBrandAsync("blue");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(b => b.Brand.Should().Contain("Blue"));
    }

    [Fact]
    public async Task GetByRoastLevelAsync_ExistingLevel_ReturnsMatchingBeans()
    {
        // Act
        var result = await _repository.GetByRoastLevelAsync(RoastLevel.Light);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Ethiopian Yirgacheffe");
    }

    [Fact]
    public async Task GetByOriginAsync_ExistingOrigin_ReturnsMatchingBeans()
    {
        // Act
        var result = await _repository.GetByOriginAsync("Ethiopia");

        // Assert
        result.Should().HaveCount(1);
        result.First().Origin.Should().Be("Ethiopia");
    }

    [Fact]
    public async Task SearchByNameAsync_PartialMatch_ReturnsMatchingBeans()
    {
        // Act
        var result = await _repository.SearchByNameAsync("ethiopian");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Ethiopian");
    }

    [Fact]
    public async Task GetFilteredAsync_MultipleCriteria_ReturnsFilteredResults()
    {
        // Act
        var result = await _repository.GetFilteredAsync(
            brand: "Blue Bottle",
            roastLevel: RoastLevel.Light);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Ethiopian Yirgacheffe");
    }

    [Fact]
    public async Task GetFilteredAsync_NoCriteria_ReturnsAllBeans()
    {
        // Act
        var result = await _repository.GetFilteredAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetRecentlyAddedAsync_ReturnsBeansInDescendingOrder()
    {
        // Act
        var result = await _repository.GetRecentlyAddedAsync(2);

        // Assert
        result.Should().HaveCount(2);
        var beansList = result.ToList();
        beansList[0].Name.Should().Be("French Roast"); // Most recent
        beansList[1].Name.Should().Be("Colombian Supremo"); // Second most recent
    }

    [Fact]
    public async Task AddAsync_ValidBean_AddsSuccessfully()
    {
        // Arrange
        var newBean = new CoffeeBean
        {
            Name = "Test Bean",
            Brand = "Test Brand",
            RoastLevel = RoastLevel.Medium,
            Origin = "Test Origin"
        };

        // Act
        var result = await _repository.AddAsync(newBean);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Bean");

        // Verify it was actually added
        var allBeans = await _repository.GetAllAsync();
        allBeans.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_ExistingBean_UpdatesSuccessfully()
    {
        // Arrange
        var bean = await _repository.GetByIdAsync(1);
        bean!.Name = "Updated Name";
        bean.ModifiedDate = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(bean);

        // Assert
        result.Name.Should().Be("Updated Name");
        result.ModifiedDate.Should().NotBeNull();

        // Verify the update persisted
        var updatedBean = await _repository.GetByIdAsync(1);
        updatedBean!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ExistingBean_DeletesSuccessfully()
    {
        // Arrange - First delete related brew sessions to avoid foreign key constraint
        var brewSessionsToDelete = _context.BrewSessions.Where(bs => bs.CoffeeBeanId == 1).ToList();
        _context.BrewSessions.RemoveRange(brewSessionsToDelete);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedBean = await _repository.GetByIdAsync(1);
        deletedBean.Should().BeNull();

        var allBeans = await _repository.GetAllAsync();
        allBeans.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsAsync_ExistingBean_ReturnsTrue()
    {
        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingBean_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
    {
        // Act
        var result = await _repository.CountAsync(b => b.Brand == "Blue Bottle");

        // Assert
        result.Should().Be(2);
    }
}