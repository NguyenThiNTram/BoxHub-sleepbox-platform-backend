using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class pricing_combo
{
    public Guid combo_id { get; set; }

    public Guid rule_id { get; set; }

    public int hours { get; set; }

    public decimal? combo_factor { get; set; }

    public bool? is_active { get; set; }

    public virtual system_price_rule rule { get; set; } = null!;
}
