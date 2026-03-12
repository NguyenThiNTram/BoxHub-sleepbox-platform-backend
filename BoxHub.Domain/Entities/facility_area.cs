using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class facility_area
{
    public Guid area_id { get; set; }

    public Guid facility_id { get; set; }

    public string area_name { get; set; } = null!;

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public virtual facility facility { get; set; } = null!;

    public virtual ICollection<sleepbox> sleepboxes { get; set; } = new List<sleepbox>();
}
