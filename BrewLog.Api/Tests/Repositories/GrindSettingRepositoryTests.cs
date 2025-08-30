using Xunit;
using FluentAssertions;
using BrewLog.Api.Repositories;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Repositories;

public class GrindSettingRepositoryTests : RepositoryTestBase
{
    private readonly GrindSettingRepository _repository;

    public GrindSettingRepositoryTests()
    {
        _repository = new GrindSettingRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectSetting()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.GrindSize.Should().Be(15);
        result.GrinderType.Should().Be("Baratza Encore");
    }

    [Fact]
    public async Task GetByGrinderTypeAsync_ExistingType_ReturnsMatchingSettings()
    {
        // Act
        var result = await _repository.GetByGrinderTypeAsync("Baratza Encore");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(gs => gs.GrinderType.Should().Be("Baratza Encore"));
    }

    [Fact]
    public async Task GetByGrinderTypeAsync_PartialMatch_ReturnsMatchingSettings()
    {
        // Act
        var result = await _repository.GetByGrinderTypeAsync("baratza");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(gs => gs.GrinderType.Should().Contain("Baratza"));
    }

    [Fact]
    public async Task GetByGrindSizeRangeAsync_ValidRange_ReturnsSettingsInRange()
    {
        // Act
        var result = await _repository.GetByGrindSizeRangeAsync(10, 20);

        // Assert
        result.Should().HaveCount(1);
        result.First().GrindSize.Should().Be(15);
    }

    [Fact]
    public async Task GetByWeightRangeAsync_ValidRange_ReturnsSettingsInRange()
    {
        // Act
        var result = await _repository.GetByWeightRangeAsync(20.0m, 25.0m);

        // Assert
        result.Should().HaveCount(1);
        result.First().GrindWeight.Should().Be(22.5m);
    }

    [Fact]
    public async Task SearchByNotesAsync_ExistingTerm_ReturnsMatchingSettings()
    {
        // Act
        var result = await _repository.SearchByNotesAsync("pour over");

        // Assert
        result.Should().HaveCount(1);
        result.First().Notes.Should().Contain("pour over");
    }

    [Fact]
    public async Task GetFilteredAsync_MultipleCriteria_ReturnsFilteredResults()
    {
        // Act
        var result = await _repository.GetFilteredAsync(
            grinderType: "Baratza",
            minGrindSize: 10,
            maxGrindSize: 20);

        // Assert
        result.Should().HaveCount(1);
        result.First().GrindSize.Should().Be(15);
    }

    [Fact]
    public async Task GetFilteredAsync_NoCriteria_ReturnsAllSettings()
    {
        // Act
        var result = await _repository.GetFilteredAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetDistinctGrinderTypesAsync_ReturnsUniqueTypes()
    {
        // Act
        var result = await _repository.GetDistinctGrinderTypesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("Baratza Encore");
        result.Should().Contain("Breville Smart Grinder");
    }

    [Fact]
    public async Task AddAsync_ValidSetting_AddsSuccessfully()
    {
        // Arrange
        var newSetting = new GrindSetting
        {
            GrindSize = 12,
            GrindTime = TimeSpan.FromSeconds(22),
            GrindWeight = 20.0m,
            GrinderType = "Test Grinder",
            Notes = "Test notes"
        };

        // Act
        var result = await _repository.AddAsync(newSetting);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.GrindSize.Should().Be(12);

        // Verify it was actually added
        var allSettings = await _repository.GetAllAsync();
        allSettings.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSetting_UpdatesSuccessfully()
    {
        // Arrange
        var setting = await _repository.GetByIdAsync(1);
        setting!.GrindSize = 16;
        setting.Notes = "Updated notes";

        // Act
        var result = await _repository.UpdateAsync(setting);

        // Assert
        result.GrindSize.Should().Be(16);
        result.Notes.Should().Be("Updated notes");

        // Verify the update persisted
        var updatedSetting = await _repository.GetByIdAsync(1);
        updatedSetting!.GrindSize.Should().Be(16);
    }

    [Fact]
    public async Task DeleteAsync_ExistingSetting_DeletesSuccessfully()
    {
        // Arrange - First delete related brew sessions to avoid foreign key constraint
        var brewSessionsToDelete = _context.BrewSessions.Where(bs => bs.GrindSettingId == 1).ToList();
        _context.BrewSessions.RemoveRange(brewSessionsToDelete);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedSetting = await _repository.GetByIdAsync(1);
        deletedSetting.Should().BeNull();

        var allSettings = await _repository.GetAllAsync();
        allSettings.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsAsync_ExistingSetting_ReturnsTrue()
    {
        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingSetting_ReturnsFalse()
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
        var result = await _repository.CountAsync(gs => gs.GrinderType.Contains("Baratza"));

        // Assert
        result.Should().Be(2);
    }
}