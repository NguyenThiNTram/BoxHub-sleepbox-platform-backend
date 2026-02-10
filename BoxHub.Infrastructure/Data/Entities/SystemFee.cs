using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class SystemFee
{
    public Guid SystemFeeId { get; set; }

    public string? FeeCode { get; set; }

    public string? FeeType { get; set; }

    public decimal? FeeValue { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool? IsActive { get; set; }
}
