using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class PlatformFeeConfig
    {
        public Guid ConfigId { get; set; }

        public FeeCode FeeCode { get; set; }

        public string FeeName { get; set; } = default!;

        public FeeType? FeeType { get; set; }

        public Guid? TargetHostId { get; set; }

        public string? TargetHostName { get; set; } // computed

        public CalcMethod CalculationMethod { get; set; }

        public decimal? PercentageValue { get; set; }

        public decimal? FixedAmount { get; set; }

        public AppliedBaseOn AppliedBaseOn { get; set; }

        public int Priority { get; set; }

        public int CalculationOrder { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
