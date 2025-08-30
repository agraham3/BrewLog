using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Models;
using System.Text.Json;

namespace BrewLog.Api.Data;

public class BrewLogDbContext(DbContextOptions<BrewLogDbContext> options) : DbContext(options)
{
    public DbSet<CoffeeBean> CoffeeBeans { get; set; } = null!;
    public DbSet<GrindSetting> GrindSettings { get; set; } = null!;
    public DbSet<BrewingEquipment> BrewingEquipment { get; set; } = null!;
    public DbSet<BrewSession> BrewSessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JSON column for equipment specifications
        modelBuilder.Entity<BrewingEquipment>()
            .Property(e => e.Specifications)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)));

        // Configure decimal precision
        modelBuilder.Entity<BrewSession>()
            .Property(b => b.WaterTemperature)
            .HasPrecision(5, 2);

        modelBuilder.Entity<GrindSetting>()
            .Property(g => g.GrindWeight)
            .HasPrecision(6, 2);

        // Configure relationships
        modelBuilder.Entity<BrewSession>()
            .HasOne(b => b.CoffeeBean)
            .WithMany(c => c.BrewSessions)
            .HasForeignKey(b => b.CoffeeBeanId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BrewSession>()
            .HasOne(b => b.GrindSetting)
            .WithMany(g => g.BrewSessions)
            .HasForeignKey(b => b.GrindSettingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BrewSession>()
            .HasOne(b => b.BrewingEquipment)
            .WithMany(e => e.BrewSessions)
            .HasForeignKey(b => b.BrewingEquipmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}