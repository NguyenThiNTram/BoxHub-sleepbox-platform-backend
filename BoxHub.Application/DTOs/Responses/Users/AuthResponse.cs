using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Users
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
