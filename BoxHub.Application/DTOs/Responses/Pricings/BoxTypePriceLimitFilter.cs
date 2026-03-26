using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class BoxTypePriceLimitFilter
    {
        public CapacityType? CapacityType { get; set; }
        public BoxClass? BoxClass { get; set; }
        public bool? IsActive { get; set; }
    }
}
