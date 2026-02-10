using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class SystemPriceRule
{
    public Guid RuleId { get; set; }

    public string? BoxType { get; set; }

    public string? PricingMode { get; set; }

    public int? MinHours { get; set; }

    public int? MaxHours { get; set; }

    public TimeOnly? FixedStartTime { get; set; }

    public TimeOnly? FixedEndTime { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public bool? IsActive { get; set; }
}
