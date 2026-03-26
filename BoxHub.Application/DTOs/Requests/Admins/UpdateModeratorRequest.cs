using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Admins
{
    public class UpdateModeratorRequest
    {
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
