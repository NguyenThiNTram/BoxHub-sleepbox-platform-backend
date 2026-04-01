using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Moderators
{
    public class AccountFilter : PagingRequest
    {
        public UserRole? Role { get; set; } // Guest | Host
        public UserStatus? Status { get; set; } // ACTIVE | SUSPENDED
        public string? Keyword { get; set; } // search username/email
    }
}
