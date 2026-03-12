using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class amenity
{
    public int amenity_id { get; set; }

    public string amenity_name { get; set; } = null!;

    public string? amenity_type { get; set; }

    public string? description { get; set; }

    public virtual ICollection<sleepbox> boxes { get; set; } = new List<sleepbox>();

    public virtual ICollection<facility> facilities { get; set; } = new List<facility>();
}
