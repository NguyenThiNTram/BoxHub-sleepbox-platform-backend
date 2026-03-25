using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Entities
{
    public class sleepbox_amenity
    {
        public Guid box_id { get; set; }
        public int amenity_id { get; set; }
        public sleepbox sleepbox { get; set; }
        public amenity amenity { get; set; }
    }
}
