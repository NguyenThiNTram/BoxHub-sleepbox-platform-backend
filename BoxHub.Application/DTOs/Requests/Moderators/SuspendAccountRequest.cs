using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Moderators
{
    public class SuspendAccountRequest
    {
        [Required(ErrorMessage = "Reason is required")]
        [MinLength(10, ErrorMessage = "Reason must be at least 10 characters")]
        public string Reason { get; set; } = string.Empty;
    }
}
