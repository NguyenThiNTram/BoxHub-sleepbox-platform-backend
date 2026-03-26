using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Entities
{
    public class system_box_type_price_limit
    {
        public Guid limit_id { get; set; }

        public CapacityType capacity_type { get; set; }
        public BoxClass box_class { get; set; }

        public decimal min_price { get; set; }
        public decimal max_price { get; set; }

        public bool is_active { get; set; }
    }
}
