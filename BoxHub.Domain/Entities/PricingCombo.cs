using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class PricingCombo
{
    public Guid ComboId { get; set; }

    public int? Hours { get; set; }

    public decimal? ComboFactor { get; set; }

    public bool? IsActive { get; set; }
}
