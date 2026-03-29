using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class AddonService
    {
        public Guid ServiceId { get; set; }

        public string ServiceName { get; set; } = null!;

        public string? Unit { get; set; }   // per booking | per night | quantity | etc.

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
