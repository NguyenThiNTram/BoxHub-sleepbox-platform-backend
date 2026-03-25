using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class dispute_attachment
{
    public Guid attachment_id { get; set; }

    public Guid dispute_id { get; set; }

    public string? attachment_role { get; set; }

    public string file_url { get; set; } = null!;

    public string? file_type { get; set; }

    public Guid? uploaded_by { get; set; }

    public DateTime uploaded_at { get; set; }

    public virtual dispute dispute { get; set; } = null!;

    public virtual user? uploaded_byNavigation { get; set; }
}
