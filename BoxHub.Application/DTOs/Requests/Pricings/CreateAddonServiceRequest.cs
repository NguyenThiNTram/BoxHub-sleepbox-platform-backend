using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class CreateAddonServiceRequest
    {
        [Required(ErrorMessage = "ServiceName is required.")]
        [MaxLength(100, ErrorMessage = "ServiceName must not exceed 100 characters.")]
        public string ServiceName { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Unit must not exceed 50 characters.")]
        public string? Unit { get; set; }

        public string? Description { get; set; }
    }
}
