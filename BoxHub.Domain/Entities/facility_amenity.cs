using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Entities
{
    public class facility_amenity
    {
        public Guid facility_id { get; set; }
        public int amenity_id { get; set; }
        public facility facility { get; set; }
        public amenity amenity { get; set; }
    }
}
