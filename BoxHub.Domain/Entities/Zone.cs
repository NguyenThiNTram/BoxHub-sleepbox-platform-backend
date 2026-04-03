using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Entities
{
    public partial class Zone
    {
        public Guid ZoneId { get; set; }
        public string ZoneCode { get; set; } = null!;
        public string ZoneName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<facility> Facilities { get; set; } = new List<facility>();
    }
}
