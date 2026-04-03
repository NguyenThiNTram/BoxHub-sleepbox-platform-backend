using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class UpdateZoneRequest
    {
        [MaxLength(100)]
        public string? ZoneName { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
