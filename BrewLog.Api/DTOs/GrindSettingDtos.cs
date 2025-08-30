namespace BrewLog.Api.DTOs;

public class GrindSettingResponseDto
{
    public int Id { get; set; }
    public int GrindSize { get; set; }
    public TimeSpan GrindTime { get; set; }
    public decimal GrindWeight { get; set; }
    public string GrinderType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class CreateGrindSettingDto
{
    public int GrindSize { get; set; }
    public TimeSpan GrindTime { get; set; }
    public decimal GrindWeight { get; set; }
    public string GrinderType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class UpdateGrindSettingDto
{
    public int GrindSize { get; set; }
    public TimeSpan GrindTime { get; set; }
    public decimal GrindWeight { get; set; }
    public string GrinderType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class GrindSettingFilterDto
{
    public int? MinGrindSize { get; set; }
    public int? MaxGrindSize { get; set; }
    public string? GrinderType { get; set; }
    public decimal? MinGrindWeight { get; set; }
    public decimal? MaxGrindWeight { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}