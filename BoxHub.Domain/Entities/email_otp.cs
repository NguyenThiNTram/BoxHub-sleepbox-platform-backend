using System;
using System.Collections.Generic;
using BoxHub.Domain.Enums;

namespace BoxHub.Domain.Entities;

public partial class email_otp
{
    public Guid otp_id { get; set; }

    public string email { get; set; } = null!;

    public string otp_code { get; set; } = null!;

    public OTPPurpose purpose { get; set; }

    public DateTimeOffset expire_at { get; set; }

    public bool is_used { get; set; } = false;

    public int attempt_count { get; set; } = 0;

    public DateTimeOffset created_at { get; set; } = DateTime.UtcNow;

    public virtual host_registration_draft? draft { get; set; }
}
