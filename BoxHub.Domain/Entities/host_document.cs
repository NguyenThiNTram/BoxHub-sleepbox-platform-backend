using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_document
{
    public Guid document_id { get; set; }

    public Guid host_id { get; set; }

    public string document_type { get; set; } = null!;

    public int version { get; set; }

    public string attachments { get; set; } = null!;

    public DateOnly? expiry_date { get; set; }

    public string? document_status { get; set; }

    public Guid? reviewed_by { get; set; }

    public DateTime? reviewed_at { get; set; }

    public string? reject_reason { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual host_profile host { get; set; } = null!;

    public virtual user? reviewed_byNavigation { get; set; }
}
