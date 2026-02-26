using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class HostAddonPrice
{
    public Guid HostServiceId { get; set; }

    public Guid? FacilityId { get; set; }

    public Guid? ServiceId { get; set; }

    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual AddonService? Service { get; set; }
}
