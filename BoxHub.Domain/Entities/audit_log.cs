using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class audit_log
{
    public Guid audit_id { get; set; }

    public Guid? actor_id { get; set; }

    public string? actor_role { get; set; }

    public string action { get; set; } = null!;

    public string? target_type { get; set; }

    public Guid? target_id { get; set; }

    public string? old_value { get; set; }

    public string? new_value { get; set; }

    public string? note { get; set; }

    public DateTime created_at { get; set; }

    public virtual user? actor { get; set; }
}
