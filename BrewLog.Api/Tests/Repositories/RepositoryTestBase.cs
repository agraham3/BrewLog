using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Repositories;

public abstract class RepositoryTestBase : IDisposable
{
    protected readonly BrewLogDbContext _context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<BrewLogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BrewLogDbContext(options);
        _context.Database.EnsureCreated();
        SeedTestData();
    }

    protected virtual void SeedTestData()
    {
        // Seed Coffee Beans
        var coffeeBeans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "Ethiopian Yirgacheffe", Brand = "Blue Bottle", RoastLevel = RoastLevel.Light, Origin = "Ethiopia", CreatedDate = DateTime.UtcNow.AddDays(-10) },
            new() { Id = 2, Name = "Colombian Supremo", Brand = "Stumptown", RoastLevel = RoastLevel.Medium, Origin = "Colombia", CreatedDate = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 3, Name = "French Roast", Brand = "Blue Bottle", RoastLevel = RoastLevel.Dark, Origin = "Brazil", CreatedDate = DateTime.UtcNow.AddDays(-2) }
        };

        // Seed Grind Settings
        var grindSettings = new List<GrindSetting>
        {
            new() { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(20), GrindWeight = 22.5m, GrinderType = "Baratza Encore", Notes = "Good for pour over", CreatedDate = DateTime.UtcNow.AddDays(-8) },
            new() { Id = 2, GrindSize = 8, GrindTime = TimeSpan.FromSeconds(18), GrindWeight = 18.0m, GrinderType = "Breville Smart Grinder", Notes = "Perfect for espresso", CreatedDate = DateTime.UtcNow.AddDays(-6) },
            new() { Id = 3, GrindSize = 25, GrindTime = TimeSpan.FromSeconds(15), GrindWeight = 30.0m, GrinderType = "Baratza Encore", Notes = "Coarse for French press", CreatedDate = DateTime.UtcNow.AddDays(-3) }
        };

        // Seed Equipment
        var equipment = new List<BrewingEquipment>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine, Specifications = new Dictionary<string, string> { {"Pressure", "15 bar"}, {"Temperature", "93°C"} }, CreatedDate = DateTime.UtcNow.AddDays(-15) },
            new() { Id = 2, Vendor = "Hario", Model = "V60", Type = EquipmentType.PourOverSetup, Specifications = new Dictionary<string, string> { {"Material", "Ceramic"}, {"Size", "02"} }, CreatedDate = DateTime.UtcNow.AddDays(-12) },
            new() { Id = 3, Vendor = "Bodum", Model = "Chambord", Type = EquipmentType.FrenchPress, Specifications = new Dictionary<string, string> { {"Capacity", "1L"}, {"Material", "Glass"} }, CreatedDate = DateTime.UtcNow.AddDays(-7) }
        };

        // Seed Brew Sessions
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, Method = BrewMethod.Espresso, WaterTemperature = 93.0m, BrewTime = TimeSpan.FromSeconds(25), TastingNotes = "Bright and fruity", Rating = 8, IsFavorite = true, CoffeeBeanId = 1, GrindSettingId = 2, BrewingEquipmentId = 1, CreatedDate = DateTime.UtcNow.AddDays(-4) },
            new() { Id = 2, Method = BrewMethod.PourOver, WaterTemperature = 96.0m, BrewTime = TimeSpan.FromMinutes(4), TastingNotes = "Smooth and balanced", Rating = 9, IsFavorite = true, CoffeeBeanId = 2, GrindSettingId = 1, BrewingEquipmentId = 2, CreatedDate = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 3, Method = BrewMethod.FrenchPress, WaterTemperature = 95.0m, BrewTime = TimeSpan.FromMinutes(4), TastingNotes = "Full bodied", Rating = 7, IsFavorite = false, CoffeeBeanId = 3, GrindSettingId = 3, BrewingEquipmentId = 3, CreatedDate = DateTime.UtcNow.AddDays(-1) }
        };

        _context.CoffeeBeans.AddRange(coffeeBeans);
        _context.GrindSettings.AddRange(grindSettings);
        _context.BrewingEquipment.AddRange(equipment);
        _context.BrewSessions.AddRange(brewSessions);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}