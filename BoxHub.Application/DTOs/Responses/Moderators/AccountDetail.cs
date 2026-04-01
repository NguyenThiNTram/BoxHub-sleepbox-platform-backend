using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Moderators
{
    public class AccountDetail
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public UserRole Role { get; set; }

        /// <summary>ACTIVE | SUSPENDED | INACTIVE</summary>
        public UserStatus Status { get; set; }

        public bool? IsEmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public GuestProfile? GuestProfile { get; set; }
        public HostInfo? HostProfile { get; set; }
    }
}
