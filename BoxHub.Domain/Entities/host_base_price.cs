using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_base_price
{
    public Guid host_price_id { get; set; }

    public Guid rule_id { get; set; }

    public Guid facility_id { get; set; }

    public string capacity_type { get; set; } = null!;

    public string box_class { get; set; } = null!;

    public decimal? base_hour_price { get; set; }

    public decimal? base_overnight_price { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual facility facility { get; set; } = null!;

    public virtual system_price_rule rule { get; set; } = null!;
}
