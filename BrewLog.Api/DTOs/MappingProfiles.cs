using AutoMapper;
using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

public class CoffeeBeanMappingProfile : Profile
{
    public CoffeeBeanMappingProfile()
    {
        CreateMap<CoffeeBean, CoffeeBeanResponseDto>();
        CreateMap<CreateCoffeeBeanDto, CoffeeBean>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
        CreateMap<UpdateCoffeeBeanDto, CoffeeBean>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
    }
}

public class GrindSettingMappingProfile : Profile
{
    public GrindSettingMappingProfile()
    {
        CreateMap<GrindSetting, GrindSettingResponseDto>();
        CreateMap<CreateGrindSettingDto, GrindSetting>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
        CreateMap<UpdateGrindSettingDto, GrindSetting>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
    }
}

public class BrewingEquipmentMappingProfile : Profile
{
    public BrewingEquipmentMappingProfile()
    {
        CreateMap<BrewingEquipment, BrewingEquipmentResponseDto>();
        CreateMap<CreateBrewingEquipmentDto, BrewingEquipment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
        CreateMap<UpdateBrewingEquipmentDto, BrewingEquipment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.BrewSessions, opt => opt.Ignore());
    }
}

public class BrewSessionMappingProfile : Profile
{
    public BrewSessionMappingProfile()
    {
        CreateMap<BrewSession, BrewSessionResponseDto>();
        CreateMap<CreateBrewSessionDto, BrewSession>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CoffeeBean, opt => opt.Ignore())
            .ForMember(dest => dest.GrindSetting, opt => opt.Ignore())
            .ForMember(dest => dest.BrewingEquipment, opt => opt.Ignore());
        CreateMap<UpdateBrewSessionDto, BrewSession>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CoffeeBean, opt => opt.Ignore())
            .ForMember(dest => dest.GrindSetting, opt => opt.Ignore())
            .ForMember(dest => dest.BrewingEquipment, opt => opt.Ignore());
    }
}

public class AnalyticsMappingProfile : Profile
{
    public AnalyticsMappingProfile()
    {
        CreateMap<BrewSession, RecentBrewSessionDto>()
            .ForMember(dest => dest.CoffeeBeanName, opt => opt.MapFrom(src => src.CoffeeBean.Name));

        CreateMap<BrewingEquipment, EquipmentPerformanceItemDto>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalUses, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.FavoriteCount, opt => opt.Ignore())
            .ForMember(dest => dest.PerformanceScore, opt => opt.Ignore());
    }
}