using AutoMapper;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Mappers
{
    public class PricingProfile : Profile
    {
        public PricingProfile()
        {
            // =========================
            // CREATE REQUEST → ENTITY
            // =========================
            CreateMap<CreateBoxTypePriceLimitRequest, system_box_type_price_limit>()
                .ForMember(dest => dest.capacity_type,
                    opt => opt.MapFrom(src => src.CapacityType.ToString()))
                .ForMember(dest => dest.box_class,
                    opt => opt.MapFrom(src => src.BoxClass.ToString()))
                .ForMember(dest => dest.min_price,
                    opt => opt.MapFrom(src => src.MinPrice))
                .ForMember(dest => dest.max_price,
                    opt => opt.MapFrom(src => src.MaxPrice))
                .ForMember(dest => dest.is_active,
                    opt => opt.Ignore()); // set ở service


            // =========================
            // ENTITY → RESPONSE
            // =========================
            CreateMap<system_box_type_price_limit, BoxTypePriceLimitResponse>()
                .ForMember(dest => dest.LimitId,
                    opt => opt.MapFrom(src => src.limit_id))
                .ForMember(dest => dest.CapacityType,
                    opt => opt.MapFrom(src => src.capacity_type))
                .ForMember(dest => dest.BoxClass,
                    opt => opt.MapFrom(src => src.box_class))
                .ForMember(dest => dest.MinPrice,
                    opt => opt.MapFrom(src => src.min_price))
                .ForMember(dest => dest.MaxPrice,
                    opt => opt.MapFrom(src => src.max_price))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.is_active));
        }
    }
}
