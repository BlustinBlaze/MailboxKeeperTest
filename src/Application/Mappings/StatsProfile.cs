using Application.DTOs.Stats;
using Application.Entities.Stats;
using AutoMapper;

namespace Application.Mappings;

public class StatsProfile : Profile
{
    public StatsProfile()
    {
        CreateMap<OrderStats, OrderStatsResponse>()
            .ForMember(dest => dest.OrdersCount, opt => opt.MapFrom(src => src.Stats.Orders))
            .ForMember(dest => dest.OrderItemsCount, opt => opt.MapFrom(src => src.Stats.OrderItems))
            .ForMember(dest => dest.OrderedFromCanadaCount, opt => opt.MapFrom(src => src.Country.Canada))
            .ForMember(dest => dest.OrderedFromOtherCountriesCount, opt => opt.MapFrom(src => src.Country.Other))
            .ReverseMap();
    }
}