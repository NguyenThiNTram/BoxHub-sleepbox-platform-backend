using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Moderators
{
    public class GuestProfile
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
