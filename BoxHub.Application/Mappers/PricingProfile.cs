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
            // ===== BOXTYPE =====
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

            // ===== RULE =====
            CreateMap<system_price_rule, PriceRuleSummary>()
                .ForMember(dest => dest.RuleId, opt => opt.MapFrom(src => src.rule_id));

            CreateMap<system_price_rule, PriceRuleDetail>()
                .IncludeBase<system_price_rule, PriceRuleSummary>();

            CreateMap<CreatePriceRuleRequest, system_price_rule>();
            CreateMap<UpdatePriceRuleRequest, system_price_rule>();

            // ===== FACTOR =====
            CreateMap<pricing_factor, PricingFactor>()
                .ForMember(dest => dest.FactorId, opt => opt.MapFrom(src => src.factor_id))
                .ReverseMap();

            // ===== COMBO =====
            CreateMap<pricing_combo, PricingCombo>()
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.combo_id))
                .ReverseMap();

            // ===== PLATFORM FEE =====

            CreateMap<CreatePlatformFeeConfigRequest, platform_fee_config>()
                .ForMember(dest => dest.fee_code,
                    opt => opt.MapFrom(src => src.FeeCode.ToString()))
                .ForMember(dest => dest.fee_name,
                    opt => opt.MapFrom(src => src.FeeName))
                .ForMember(dest => dest.fee_type,
                    opt => opt.MapFrom(src => src.FeeType.HasValue ? src.FeeType.ToString() : null))
                .ForMember(dest => dest.target_host_id,
                    opt => opt.MapFrom(src => src.TargetHostId))
                .ForMember(dest => dest.calculation_method,
                    opt => opt.MapFrom(src => src.CalculationMethod.ToString()))
                .ForMember(dest => dest.percentage_value,
                    opt => opt.MapFrom(src => src.PercentageValue))
                .ForMember(dest => dest.fixed_amount,
                    opt => opt.MapFrom(src => src.FixedAmount))
                .ForMember(dest => dest.applied_base_on,
                    opt => opt.MapFrom(src => src.AppliedBaseOn.ToString()))
                .ForMember(dest => dest.calculation_order,
                    opt => opt.MapFrom(src => src.CalculationOrder))
                .ForMember(dest => dest.effective_from,
                    opt => opt.MapFrom(src => src.EffectiveFrom))
                .ForMember(dest => dest.effective_to,
                    opt => opt.MapFrom(src => src.EffectiveTo))
                .ForMember(dest => dest.is_active,
                    opt => opt.Ignore())
                .ForMember(dest => dest.created_at,
                    opt => opt.Ignore());


            // ENTITY → RESPONSE
            CreateMap<platform_fee_config, PlatformFeeConfig>()
                .ForMember(dest => dest.ConfigId,
                    opt => opt.MapFrom(src => src.config_id))
                .ForMember(dest => dest.FeeCode,
                    opt => opt.MapFrom(src => src.fee_code))
                .ForMember(dest => dest.FeeName,
                    opt => opt.MapFrom(src => src.fee_name))
                .ForMember(dest => dest.FeeType,
                    opt => opt.MapFrom(src => src.fee_type != null ? src.fee_type : (FeeType?)null))
                .ForMember(dest => dest.TargetHostId,
                    opt => opt.MapFrom(src => src.target_host_id))
                .ForMember(dest => dest.CalculationMethod,
                    opt => opt.MapFrom(src => src.calculation_method))
                .ForMember(dest => dest.PercentageValue,
                    opt => opt.MapFrom(src => src.percentage_value))
                .ForMember(dest => dest.FixedAmount,
                    opt => opt.MapFrom(src => src.fixed_amount))
                .ForMember(dest => dest.AppliedBaseOn,
                    opt => opt.MapFrom(src => src.applied_base_on))
                .ForMember(dest => dest.CalculationOrder,
                    opt => opt.MapFrom(src => src.calculation_order))
                .ForMember(dest => dest.EffectiveFrom,
                    opt => opt.MapFrom(src => src.effective_from))
                .ForMember(dest => dest.EffectiveTo,
                    opt => opt.MapFrom(src => src.effective_to))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.is_active))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.created_at))
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => src.priority))

                // computed
                .ForMember(dest => dest.TargetHostName,
                    opt => opt.Ignore());

            // ===== ADDON SERVICE =====

            CreateMap<addon_service, AddonService>()
                .ForMember(dest => dest.ServiceId,
                    opt => opt.MapFrom(src => src.service_id))
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.service_name))
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src => src.unit))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.is_active));

            CreateMap<CreateAddonServiceRequest, addon_service>()
                .ForMember(dest => dest.service_id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.service_name,
                    opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.unit,
                    opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.description,
                    opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.is_active,
                    opt => opt.Ignore())
                .ForMember(dest => dest.created_at,
                    opt => opt.Ignore());


            CreateMap<UpdateAddonServiceRequest, addon_service>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
