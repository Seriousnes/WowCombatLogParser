using AutoMapper;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Utility;

public class MappingProfiles : Profile
{
    public MappingProfiles() 
    { 
        CreateMap<Unit, Actor>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id))
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
            .ForMember(dest => dest.UnitType, src => src.Ignore())
            ;

        CreateMap<Unit, Player>()
            .IncludeBase<Unit, Actor>()
            .ForMember(dest => dest.UnitType, src => src.MapFrom(x => UnitType.Player));
    }
}
