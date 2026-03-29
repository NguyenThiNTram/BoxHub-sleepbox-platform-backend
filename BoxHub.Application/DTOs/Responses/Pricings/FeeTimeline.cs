using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class FeeTimeline
    {
        public FeeCode FeeCode { get; set; }

        public List<PlatformFeeConfig> History { get; set; } = new();

        public PlatformFeeConfig? Current { get; set; }
    }
}
