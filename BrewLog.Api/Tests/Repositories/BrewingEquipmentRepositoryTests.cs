using Xunit;
using FluentAssertions;
using BrewLog.Api.Repositories;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Repositories;

public class BrewingEquipmentRepositoryTests : RepositoryTestBase
{
    private readonly BrewingEquipmentRepository _repository;

    public BrewingEquipmentRepositoryTests()
    {
        _repository = new BrewingEquipmentRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectEquipment()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Vendor.Should().Be("Breville");
        result.Model.Should().Be("Barista Express");
        result.Type.Should().Be(EquipmentType.EspressoMachine);
    }

    [Fact]
    public async Task GetByTypeAsync_ExistingType_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetByTypeAsync(EquipmentType.EspressoMachine);

        // Assert
        result.Should().HaveCount(1);
        result.First().Type.Should().Be(EquipmentType.EspressoMachine);
    }

    [Fact]
    public async Task GetByVendorAsync_ExistingVendor_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetByVendorAsync("Breville");

        // Assert
        result.Should().HaveCount(1);
        result.First().Vendor.Should().Be("Breville");
    }

    [Fact]
    public async Task GetByVendorAsync_PartialMatch_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetByVendorAsync("breville");

        // Assert
        result.Should().HaveCount(1);
        result.First().Vendor.Should().Contain("Breville");
    }

    [Fact]
    public async Task GetByModelAsync_ExistingModel_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetByModelAsync("V60");

        // Assert
        result.Should().HaveCount(1);
        result.First().Model.Should().Be("V60");
    }

    [Fact]
    public async Task SearchByVendorOrModelAsync_MatchesVendor_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.SearchByVendorOrModelAsync("hario");

        // Assert
        result.Should().HaveCount(1);
        result.First().Vendor.Should().Be("Hario");
    }

    [Fact]
    public async Task SearchByVendorOrModelAsync_MatchesModel_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.SearchByVendorOrModelAsync("chambord");

        // Assert
        result.Should().HaveCount(1);
        result.First().Model.Should().Be("Chambord");
    }

    [Fact]
    public async Task GetFilteredAsync_MultipleCriteria_ReturnsFilteredResults()
    {
        // Act
        var result = await _repository.GetFilteredAsync(
            type: EquipmentType.PourOverSetup,
            vendor: "Hario");

        // Assert
        result.Should().HaveCount(1);
        result.First().Vendor.Should().Be("Hario");
        result.First().Type.Should().Be(EquipmentType.PourOverSetup);
    }

    [Fact]
    public async Task GetFilteredAsync_NoCriteria_ReturnsAllEquipment()
    {
        // Act
        var result = await _repository.GetFilteredAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetDistinctVendorsAsync_ReturnsUniqueVendors()
    {
        // Act
        var result = await _repository.GetDistinctVendorsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain("Breville");
        result.Should().Contain("Hario");
        result.Should().Contain("Bodum");
    }

    [Fact]
    public async Task GetDistinctModelsAsync_ReturnsUniqueModels()
    {
        // Act
        var result = await _repository.GetDistinctModelsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain("Barista Express");
        result.Should().Contain("V60");
        result.Should().Contain("Chambord");
    }

    [Fact]
    public async Task GetBySpecificationAsync_ExistingKeyValue_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetBySpecificationAsync("Pressure", "15 bar");

        // Assert
        result.Should().HaveCount(1);
        result.First().Vendor.Should().Be("Breville");
    }

    [Fact]
    public async Task GetBySpecificationAsync_PartialValueMatch_ReturnsMatchingEquipment()
    {
        // Act
        var result = await _repository.GetBySpecificationAsync("Material", "ceramic");

        // Assert
        result.Should().HaveCount(1);
        result.First().Model.Should().Be("V60");
    }

    [Fact]
    public async Task AddAsync_ValidEquipment_AddsSuccessfully()
    {
        // Arrange
        var newEquipment = new BrewingEquipment
        {
            Vendor = "Test Vendor",
            Model = "Test Model",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string> { { "Burr Type", "Ceramic" } }
        };

        // Act
        var result = await _repository.AddAsync(newEquipment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Vendor.Should().Be("Test Vendor");

        // Verify it was actually added
        var allEquipment = await _repository.GetAllAsync();
        allEquipment.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_ExistingEquipment_UpdatesSuccessfully()
    {
        // Arrange
        var equipment = await _repository.GetByIdAsync(1);
        equipment!.Model = "Updated Model";
        equipment.Specifications["NewSpec"] = "NewValue";

        // Act
        var result = await _repository.UpdateAsync(equipment);

        // Assert
        result.Model.Should().Be("Updated Model");
        result.Specifications.Should().ContainKey("NewSpec");

        // Verify the update persisted
        var updatedEquipment = await _repository.GetByIdAsync(1);
        updatedEquipment!.Model.Should().Be("Updated Model");
    }

    [Fact]
    public async Task DeleteAsync_ExistingEquipment_DeletesSuccessfully()
    {
        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedEquipment = await _repository.GetByIdAsync(1);
        deletedEquipment.Should().BeNull();

        var allEquipment = await _repository.GetAllAsync();
        allEquipment.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsAsync_ExistingEquipment_ReturnsTrue()
    {
        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingEquipment_ReturnsFalse()
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
        var result = await _repository.CountAsync(e => e.Type == EquipmentType.EspressoMachine);

        // Assert
        result.Should().Be(1);
    }
}