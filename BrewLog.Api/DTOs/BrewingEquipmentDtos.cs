using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

public class BrewingEquipmentResponseDto
{
    public int Id { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public Dictionary<string, string> Specifications { get; set; } = [];
    public DateTime CreatedDate { get; set; }
}

public class CreateBrewingEquipmentDto
{
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public Dictionary<string, string> Specifications { get; set; } = [];
}

public class UpdateBrewingEquipmentDto
{
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public Dictionary<string, string> Specifications { get; set; } = [];
}

public class BrewingEquipmentFilterDto
{
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public EquipmentType? Type { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}