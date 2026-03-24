using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Admins
{
    public class UserItemResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
