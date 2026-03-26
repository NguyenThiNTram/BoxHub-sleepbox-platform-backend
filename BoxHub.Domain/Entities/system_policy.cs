using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class system_policy
{
    public Guid policy_id { get; set; }

    public string policy_type { get; set; } = null!;

    public string title { get; set; } = null!;

    public string? content { get; set; }

    public string? version { get; set; }

    public DateTime? effective_date { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }
}
