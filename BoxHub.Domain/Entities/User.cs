using System;
using System.Collections.Generic;
using BoxHub.Domain.Enums;

namespace BoxHub.Domain.Entities;

public partial class user
{
    public Guid user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? phone { get; set; }

    public string password_hash { get; set; } = null!;

    public UserRole role { get; set; }

    public UserStatus user_status { get; set; }

    public bool? is_email_verified { get; set; }

    public DateTime? email_verified_at { get; set; }

    public DateTime? last_login_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public virtual ICollection<audit_log> audit_logs { get; set; } = new List<audit_log>();

    public virtual ICollection<booking> bookingcancelled_bies { get; set; } = new List<booking>();

    public virtual ICollection<booking> bookingguests { get; set; } = new List<booking>();

    public virtual ICollection<conversation> conversations { get; set; } = new List<conversation>();

    public virtual ICollection<dispute_attachment> dispute_attachments { get; set; } = new List<dispute_attachment>();

    public virtual ICollection<dispute> disputeadmin_approvals { get; set; } = new List<dispute>();

    public virtual ICollection<dispute> disputeassigned_moderators { get; set; } = new List<dispute>();

    public virtual ICollection<dispute> disputeraised_byNavigations { get; set; } = new List<dispute>();

    public virtual ICollection<host_document> host_documents { get; set; } = new List<host_document>();

    public virtual host_profile? host_profileuser { get; set; }

    public virtual ICollection<host_profile> host_profileverified_byNavigations { get; set; } = new List<host_profile>();

    public virtual ICollection<media_asset> media_assets { get; set; } = new List<media_asset>();

    public virtual ICollection<message> messages { get; set; } = new List<message>();

    public virtual ICollection<notification> notifications { get; set; } = new List<notification>();

    public virtual ICollection<review> reviews { get; set; } = new List<review>();

    public virtual ICollection<staff_profile> staff_profiles { get; set; } = new List<staff_profile>();

    public virtual ICollection<user_favorite> user_favorites { get; set; } = new List<user_favorite>();

    public virtual user_profile? user_profile { get; set; }

    public virtual ICollection<facility_document> facility_documents { get; set; } = new List<facility_document>();
}
