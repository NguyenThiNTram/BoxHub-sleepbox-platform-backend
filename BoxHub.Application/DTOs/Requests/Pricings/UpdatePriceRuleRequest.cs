using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class UpdatePriceRuleRequest
    {
        public int? MinHours { get; set; }

        public int? MaxHours { get; set; }

        public TimeSpan? FixedStartTime { get; set; }
        public TimeSpan? FixedEndTime { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int? Priority { get; set; }

        public bool? IsActive { get; set; }
    }
}
