using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

public class BrewSessionResponseDto
{
    public int Id { get; set; }
    public BrewMethod Method { get; set; }
    public decimal WaterTemperature { get; set; }
    public TimeSpan BrewTime { get; set; }
    public string TastingNotes { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedDate { get; set; }

    // Foreign Keys
    public int CoffeeBeanId { get; set; }
    public int GrindSettingId { get; set; }
    public int? BrewingEquipmentId { get; set; }

    // Related entities
    public CoffeeBeanResponseDto CoffeeBean { get; set; } = null!;
    public GrindSettingResponseDto GrindSetting { get; set; } = null!;
    public BrewingEquipmentResponseDto? BrewingEquipment { get; set; }
}

public class CreateBrewSessionDto
{
    public BrewMethod Method { get; set; }
    public decimal WaterTemperature { get; set; }
    public TimeSpan BrewTime { get; set; }
    public string TastingNotes { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public bool IsFavorite { get; set; }

    // Foreign Keys
    public int CoffeeBeanId { get; set; }
    public int GrindSettingId { get; set; }
    public int? BrewingEquipmentId { get; set; }
}

public class UpdateBrewSessionDto
{
    public BrewMethod Method { get; set; }
    public decimal WaterTemperature { get; set; }
    public TimeSpan BrewTime { get; set; }
    public string TastingNotes { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public bool IsFavorite { get; set; }

    // Foreign Keys
    public int CoffeeBeanId { get; set; }
    public int GrindSettingId { get; set; }
    public int? BrewingEquipmentId { get; set; }
}

public class BrewSessionFilterDto
{
    public BrewMethod? Method { get; set; }
    public int? CoffeeBeanId { get; set; }
    public int? GrindSettingId { get; set; }
    public int? BrewingEquipmentId { get; set; }
    public decimal? MinWaterTemperature { get; set; }
    public decimal? MaxWaterTemperature { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public bool? IsFavorite { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}