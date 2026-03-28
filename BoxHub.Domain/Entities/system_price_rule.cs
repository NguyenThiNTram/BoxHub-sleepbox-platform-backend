using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class system_price_rule
{
    public Guid rule_id { get; set; }

    public int? priority { get; set; }

    public PricingMode pricing_mode { get; set; }

    public int? min_hours { get; set; }

    public int? max_hours { get; set; }

    public TimeOnly? fixed_start_time { get; set; }

    public TimeOnly? fixed_end_time { get; set; }

    public decimal? min_price { get; set; }

    public decimal? max_price { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual ICollection<host_base_price> host_base_prices { get; set; } = new List<host_base_price>();

    public virtual ICollection<pricing_combo> pricing_combos { get; set; } = new List<pricing_combo>();

    public virtual ICollection<pricing_factor> pricing_factors { get; set; } = new List<pricing_factor>();
}
