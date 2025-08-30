using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

public class BrewSession
{
    public int Id { get; set; }
    
    public BrewMethod Method { get; set; }
    
    [Range(60.0, 100.0)]
    public decimal WaterTemperature { get; set; }
    
    public TimeSpan BrewTime { get; set; }
    
    [MaxLength(1000)]
    public string TastingNotes { get; set; } = string.Empty;
    
    [Range(1, 10)]
    public int? Rating { get; set; }
    
    public bool IsFavorite { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int CoffeeBeanId { get; set; }
    public int GrindSettingId { get; set; }
    public int? BrewingEquipmentId { get; set; }
    
    // Navigation properties
    public CoffeeBean CoffeeBean { get; set; } = null!;
    public GrindSetting GrindSetting { get; set; } = null!;
    public BrewingEquipment? BrewingEquipment { get; set; }
}