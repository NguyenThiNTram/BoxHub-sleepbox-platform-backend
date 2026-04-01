using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Moderators
{
    public class AccountListItem
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public UserRole Role { get; set; }

        // ACTIVE | SUSPENDED | INACTIVE
        public UserStatus Status { get; set; }

        public bool? IsEmailVerified { get; set; }

        //public string? AvatarUrl { get; set; }

        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // --- Host fields ---
        public string? RepresentativeName { get; set; }
        public string? VerifiedStatus { get; set; }
    }
}
