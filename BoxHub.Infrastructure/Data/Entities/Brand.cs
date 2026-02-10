using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Brand
{
    public Guid BrandId { get; set; }

    public Guid? HostId { get; set; }

    public string? BrandName { get; set; }

    public string? BrandAvatar { get; set; }

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    public virtual HostProfile? Host { get; set; }
}
