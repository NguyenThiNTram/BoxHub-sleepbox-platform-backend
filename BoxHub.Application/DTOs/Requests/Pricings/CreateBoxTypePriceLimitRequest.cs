using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class CreateBoxTypePriceLimitRequest
    {
        public CapacityType CapacityType { get; set; }
        public BoxClass BoxClass { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
