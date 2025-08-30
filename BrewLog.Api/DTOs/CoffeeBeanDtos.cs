using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

public class CoffeeBeanResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public RoastLevel RoastLevel { get; set; }
    public string Origin { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class CreateCoffeeBeanDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public RoastLevel RoastLevel { get; set; }
    public string Origin { get; set; } = string.Empty;
}

public class UpdateCoffeeBeanDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public RoastLevel RoastLevel { get; set; }
    public string Origin { get; set; } = string.Empty;
}

public class CoffeeBeanFilterDto
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public RoastLevel? RoastLevel { get; set; }
    public string? Origin { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}