using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string? AmenityName { get; set; }

    public string? AmenityType { get; set; }

    public string? IconUrl { get; set; }

    public string? AmenityScope { get; set; }

    public string? AmenityCategory { get; set; }

    public virtual ICollection<Sleepbox> Boxes { get; set; } = new List<Sleepbox>();
}
