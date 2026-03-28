using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class PricingFactor
    {
        public Guid FactorId { get; set; }

        public FactorType FactorType { get; set; }

        public string? RefCode { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public decimal BaseFactor { get; set; }

        public decimal? MinFactor { get; set; }
        public decimal? MaxFactor { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }
}
