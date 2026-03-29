using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class CreatePlatformFeeConfigRequest
    {
        [Required]
        public FeeCode FeeCode { get; set; }

        [Required]
        public string FeeName { get; set; } = default!;

        public FeeType? FeeType { get; set; }

        public Guid? TargetHostId { get; set; }

        [Required]
        public CalcMethod CalculationMethod { get; set; }

        public decimal? PercentageValue { get; set; }

        public decimal? FixedAmount { get; set; }

        [Required]
        public AppliedBaseOn AppliedBaseOn { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public int CalculationOrder { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
