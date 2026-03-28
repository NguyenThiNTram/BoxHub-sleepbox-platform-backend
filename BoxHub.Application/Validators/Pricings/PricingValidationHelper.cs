using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Validators.Pricings
{
    public static class PricingValidationHelper
    {
        // create
        public static string? ValidateCreate(CreatePriceRuleRequest req)
        {
            if (req.PricingMode == PricingMode.HOURLY)
            {
                if (!req.MinHours.HasValue || !req.MaxHours.HasValue)
                    return "MinHours/MaxHours required";

                if (req.MaxHours <= req.MinHours)
                    return "MaxHours must be greater than MinHours";
            }

            if (req.PricingMode == PricingMode.OVERNIGHT)
            {
                if (!req.FixedStartTime.HasValue || !req.FixedEndTime.HasValue)
                    return "FixedStartTime/EndTime required";
            }

            return null;
        }

        // update
        public static string? ValidateUpdate(system_price_rule entity, UpdatePriceRuleRequest req)
        {
            if (entity.pricing_mode == PricingMode.HOURLY)
            {
                if (req.MinHours.HasValue && req.MaxHours.HasValue &&
                    req.MaxHours <= req.MinHours)
                    return "Invalid hours range";
            }

            return null;
        }

        // combo
        public static string? ValidateCombo(List<pricing_combo> existingCombos, int hours)
        {
            if (existingCombos.Any(x => x.hours == hours))
                return $"Combo with {hours} hours already exists";

            if (hours <= 0)
                return "Hours must be greater than 0";

            return null;
        }

        // factor
        public static string? ValidateFactor(pricing_factor factor)
        {
            if (factor.min_factor.HasValue && factor.max_factor.HasValue)
            {
                if (factor.max_factor < factor.min_factor)
                    return "max_factor must be >= min_factor";
            }

            if (factor.base_factor < 0)
                return "base_factor must be >= 0";

            return null;
        }
    }
}
