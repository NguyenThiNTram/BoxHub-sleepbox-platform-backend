using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class media_asset
{
    public Guid media_id { get; set; }

    public Guid target_id { get; set; }

    public string target_type { get; set; } = null!;

    public string media_type { get; set; } = null!;

    public string media_url { get; set; } = null!;

    public string? thumbnail_url { get; set; }

    public Guid? uploaded_by { get; set; }

    public int? display_order { get; set; }

    public bool? is_cover { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual user? uploaded_byNavigation { get; set; }
}
