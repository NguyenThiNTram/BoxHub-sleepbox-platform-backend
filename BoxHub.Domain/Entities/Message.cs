using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class message
{
    public Guid message_id { get; set; }

    public Guid conversation_id { get; set; }

    public Guid sender_id { get; set; }

    public string? message_type { get; set; }

    public string? content { get; set; }

    public DateTime sent_at { get; set; }

    public bool? is_read { get; set; }

    public virtual conversation conversation { get; set; } = null!;

    public virtual user sender { get; set; } = null!;
}
