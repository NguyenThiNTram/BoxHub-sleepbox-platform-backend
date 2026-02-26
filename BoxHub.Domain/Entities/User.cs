using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    //public string? Role { get; set; }
    public UserRole Role { get; set; }

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string UserStatus { get; set; } = null!;

    public bool? IsEmailVerified { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Booking> BookingCancelledBies { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingGuests { get; set; } = new List<Booking>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Dispute> DisputeAdminApprovals { get; set; } = new List<Dispute>();

    public virtual ICollection<Dispute> DisputeAssignedModerators { get; set; } = new List<Dispute>();

    public virtual ICollection<DisputeAttachment> DisputeAttachments { get; set; } = new List<DisputeAttachment>();

    public virtual ICollection<Dispute> DisputeRaisedByNavigations { get; set; } = new List<Dispute>();

    public virtual ICollection<HostDocument> HostDocuments { get; set; } = new List<HostDocument>();

    public virtual HostProfile? HostProfile { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<StaffProfile> StaffProfiles { get; set; } = new List<StaffProfile>();

    public virtual UserProfile? UserProfile { get; set; }
}
