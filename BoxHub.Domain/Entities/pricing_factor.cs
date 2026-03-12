using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class pricing_factor
{
    public Guid factor_id { get; set; }

    public Guid rule_id { get; set; }

    public string factor_type { get; set; } = null!;

    public string? ref_code { get; set; }

    public TimeOnly? start_time { get; set; }

    public TimeOnly? end_time { get; set; }

    public decimal? min_factor { get; set; }

    public decimal? max_factor { get; set; }

    public int? priority { get; set; }

    public bool? is_active { get; set; }

    public virtual system_price_rule rule { get; set; } = null!;
}
