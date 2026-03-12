using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class notification
{
    public Guid notification_id { get; set; }

    public Guid user_id { get; set; }

    public string notification_type { get; set; } = null!;

    public string? target_type { get; set; }

    public Guid? target_id { get; set; }

    public string? title { get; set; }

    public string? content { get; set; }

    public DateTime created_at { get; set; }

    public virtual user user { get; set; } = null!;
}
