using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class dispute
{
    public Guid dispute_id { get; set; }

    public Guid booking_id { get; set; }

    public Guid raised_by { get; set; }

    public string dispute_type { get; set; } = null!;

    public string? description { get; set; }

    public string? status { get; set; }

    public Guid? assigned_moderator_id { get; set; }

    public string? moderator_note { get; set; }

    public string? resolution_type { get; set; }

    public decimal? refund_amount { get; set; }

    public Guid? admin_approval_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? resolved_at { get; set; }

    public virtual user? admin_approval { get; set; }

    public virtual user? assigned_moderator { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual ICollection<dispute_attachment> dispute_attachments { get; set; } = new List<dispute_attachment>();

    public virtual user raised_byNavigation { get; set; } = null!;
}
