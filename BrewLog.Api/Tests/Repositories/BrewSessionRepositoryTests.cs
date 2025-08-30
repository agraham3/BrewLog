using Xunit;
using FluentAssertions;
using BrewLog.Api.Repositories;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Repositories;

public class BrewSessionRepositoryTests : RepositoryTestBase
{
    private readonly BrewSessionRepository _repository;

    public BrewSessionRepositoryTests()
    {
        _repository = new BrewSessionRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCorrectSession()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Method.Should().Be(BrewMethod.Espresso);
        result.Rating.Should().Be(8);
        result.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdWithIncludesAsync_ExistingId_ReturnsSessionWithRelatedData()
    {
        // Act
        var result = await _repository.GetByIdWithIncludesAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.CoffeeBean.Should().NotBeNull();
        result.CoffeeBean.Name.Should().Be("Ethiopian Yirgacheffe");
        result.GrindSetting.Should().NotBeNull();
        result.GrindSetting.GrindSize.Should().Be(8);
        result.BrewingEquipment.Should().NotBeNull();
        result.BrewingEquipment!.Vendor.Should().Be("Breville");
    }

    [Fact]
    public async Task GetByBrewMethodAsync_ExistingMethod_ReturnsMatchingSessions()
    {
        // Act
        var result = await _repository.GetByBrewMethodAsync(BrewMethod.Espresso);

        // Assert
        result.Should().HaveCount(1);
        result.First().Method.Should().Be(BrewMethod.Espresso);
        result.First().CoffeeBean.Should().NotBeNull(); // Should include related data
    }

    [Fact]
    public async Task GetByCoffeeBeanIdAsync_ExistingBeanId_ReturnsMatchingSessions()
    {
        // Act
        var result = await _repository.GetByCoffeeBeanIdAsync(1);

        // Assert
        result.Should().HaveCount(1);
        result.First().CoffeeBeanId.Should().Be(1);
        result.First().CoffeeBean.Name.Should().Be("Ethiopian Yirgacheffe");
    }

    [Fact]
    public async Task GetByGrindSettingIdAsync_ExistingSettingId_ReturnsMatchingSessions()
    {
        // Act
        var result = await _repository.GetByGrindSettingIdAsync(2);

        // Assert
        result.Should().HaveCount(1);
        result.First().GrindSettingId.Should().Be(2);
        result.First().GrindSetting.GrindSize.Should().Be(8);
    }

    [Fact]
    public async Task GetByEquipmentIdAsync_ExistingEquipmentId_ReturnsMatchingSessions()
    {
        // Act
        var result = await _repository.GetByEquipmentIdAsync(1);

        // Assert
        result.Should().HaveCount(1);
        result.First().BrewingEquipmentId.Should().Be(1);
        result.First().BrewingEquipment!.Vendor.Should().Be("Breville");
    }

    [Fact]
    public async Task GetFavoritesAsync_ReturnsFavoriteSessions()
    {
        // Act
        var result = await _repository.GetFavoritesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(s => s.IsFavorite.Should().BeTrue());
    }

    [Fact]
    public async Task GetByRatingRangeAsync_ValidRange_ReturnsSessionsInRange()
    {
        // Act
        var result = await _repository.GetByRatingRangeAsync(8, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(s => s.Rating.Should().BeGreaterOrEqualTo(8));
    }

    [Fact]
    public async Task GetByDateRangeAsync_ValidRange_ReturnsSessionsInRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-3);
        var endDate = DateTime.UtcNow;

        // Act
        var result = await _repository.GetByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(2); // Sessions from last 3 days
    }

    [Fact]
    public async Task GetRecentAsync_ReturnsSessionsInDescendingOrder()
    {
        // Act
        var result = await _repository.GetRecentAsync(2);

        // Assert
        result.Should().HaveCount(2);
        var sessionsList = result.ToList();
        sessionsList[0].CreatedDate.Should().BeAfter(sessionsList[1].CreatedDate);
    }

    [Fact]
    public async Task GetTopRatedAsync_ReturnsHighestRatedSessions()
    {
        // Act
        var result = await _repository.GetTopRatedAsync(2);

        // Assert
        result.Should().HaveCount(2);
        var sessionsList = result.ToList();
        sessionsList[0].Rating.Should().BeGreaterOrEqualTo(sessionsList[1].Rating!.Value);
    }

    [Fact]
    public async Task SearchByTastingNotesAsync_ExistingTerm_ReturnsMatchingSessions()
    {
        // Act
        var result = await _repository.SearchByTastingNotesAsync("bright");

        // Assert
        result.Should().HaveCount(1);
        result.First().TastingNotes.Should().Contain("Bright");
    }

    [Fact]
    public async Task GetFilteredAsync_MultipleCriteria_ReturnsFilteredResults()
    {
        // Act
        var result = await _repository.GetFilteredAsync(
            method: BrewMethod.Espresso,
            isFavorite: true,
            minRating: 8);

        // Assert
        result.Should().HaveCount(1);
        result.First().Method.Should().Be(BrewMethod.Espresso);
        result.First().IsFavorite.Should().BeTrue();
        result.First().Rating.Should().BeGreaterOrEqualTo(8);
    }

    [Fact]
    public async Task GetFilteredAsync_NoCriteria_ReturnsAllSessions()
    {
        // Act
        var result = await _repository.GetFilteredAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetWithIncludesAsync_ReturnsAllSessionsWithRelatedData()
    {
        // Act
        var result = await _repository.GetWithIncludesAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(s =>
        {
            s.CoffeeBean.Should().NotBeNull();
            s.GrindSetting.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task AddAsync_ValidSession_AddsSuccessfully()
    {
        // Arrange
        var newSession = new BrewSession
        {
            Method = BrewMethod.Drip,
            WaterTemperature = 94.0m,
            BrewTime = TimeSpan.FromMinutes(5),
            TastingNotes = "Test notes",
            Rating = 6,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 2
        };

        // Act
        var result = await _repository.AddAsync(newSession);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Method.Should().Be(BrewMethod.Drip);

        // Verify it was actually added
        var allSessions = await _repository.GetAllAsync();
        allSessions.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSession_UpdatesSuccessfully()
    {
        // Arrange
        var session = await _repository.GetByIdAsync(1);
        session!.Rating = 10;
        session.TastingNotes = "Updated notes";

        // Act
        var result = await _repository.UpdateAsync(session);

        // Assert
        result.Rating.Should().Be(10);
        result.TastingNotes.Should().Be("Updated notes");

        // Verify the update persisted
        var updatedSession = await _repository.GetByIdAsync(1);
        updatedSession!.Rating.Should().Be(10);
    }

    [Fact]
    public async Task DeleteAsync_ExistingSession_DeletesSuccessfully()
    {
        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedSession = await _repository.GetByIdAsync(1);
        deletedSession.Should().BeNull();

        var allSessions = await _repository.GetAllAsync();
        allSessions.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsAsync_ExistingSession_ReturnsTrue()
    {
        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingSession_ReturnsFalse()
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
        var result = await _repository.CountAsync(s => s.IsFavorite);

        // Assert
        result.Should().Be(2);
    }
}