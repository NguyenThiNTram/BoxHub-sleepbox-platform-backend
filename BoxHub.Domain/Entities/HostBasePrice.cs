using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class HostBasePrice
{
    public Guid HostPriceId { get; set; }

    public Guid? FacilityId { get; set; }

    public string? BoxType { get; set; }

    public decimal? BaseHourPrice { get; set; }

    public decimal? BaseOvernightPrice { get; set; }

    public string? AppliedLocationCode { get; set; }

    public bool? IsActive { get; set; }

    public virtual Facility? Facility { get; set; }
}
