using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_registration_draft
{
    public Guid draft_id { get; set; }

    public string email { get; set; } = null!;

    public string? phone { get; set; }

    public Guid? otp_id { get; set; }

    public string payload { get; set; } = null!;

    public bool is_verified { get; set; } = false;

    public DateTime expire_at { get; set; }

    public DateTime? created_at { get; set; } = DateTime.UtcNow;

    public DateTime? updated_at { get; set; }

    public virtual email_otp? otp { get; set; }
}
