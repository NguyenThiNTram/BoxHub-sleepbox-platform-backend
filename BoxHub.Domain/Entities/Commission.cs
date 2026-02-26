using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class Commission
{
    public Guid CommissionId { get; set; }

    public decimal? Percentage { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<HostPayout> HostPayouts { get; set; } = new List<HostPayout>();
}
