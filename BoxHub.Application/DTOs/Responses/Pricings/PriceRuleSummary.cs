using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class PriceRuleSummary
    {
        public Guid RuleId { get; set; }

        public PricingMode PricingMode { get; set; }

        public int? MinHours { get; set; }
        public int? MaxHours { get; set; }

        public TimeSpan? FixedStartTime { get; set; }
        public TimeSpan? FixedEndTime { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }

        // Computed
        public int FactorCount { get; set; }
        public int ComboCount { get; set; }

        //FactorCount, ComboCount = computed(service xử lý)
        //Không include factors/combo để tối ưu performance
    }
}
