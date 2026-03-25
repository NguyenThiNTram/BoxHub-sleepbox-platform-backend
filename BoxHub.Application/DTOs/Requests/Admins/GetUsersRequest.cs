using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Admins
{
    public class GetUsersRequest : PagingRequest
    {
        public UserRole? Role { get; set; }
        public UserStatus? UserStatus { get; set; }
        public string? SearchTerm { get; set; }

        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
