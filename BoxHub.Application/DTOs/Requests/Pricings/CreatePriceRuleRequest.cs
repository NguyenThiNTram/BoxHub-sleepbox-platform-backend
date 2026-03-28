using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class CreatePriceRuleRequest
    {
        [Required]
        public PricingMode PricingMode { get; set; }

        // HOURLY
        public int? MinHours { get; set; }
        public int? MaxHours { get; set; }

        // OVERNIGHT
        public TimeSpan? FixedStartTime { get; set; }
        public TimeSpan? FixedEndTime { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int Priority { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
