using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class PricingFactor
{
    public Guid FactorId { get; set; }

    public string? FactorType { get; set; }

    public string? RefCode { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public decimal? MinFactor { get; set; }

    public decimal? MaxFactor { get; set; }

    public int? Priority { get; set; }

    public bool? IsActive { get; set; }
}
