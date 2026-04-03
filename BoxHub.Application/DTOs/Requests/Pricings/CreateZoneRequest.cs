using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class CreateZoneRequest
    {
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Z0-9_]+$",
           ErrorMessage = "zone_code only uses uppercase letters, numbers, and underscores.")]
        public string ZoneCode { get; set; } = null!;

        [Required, MaxLength(100)]
        public string ZoneName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
