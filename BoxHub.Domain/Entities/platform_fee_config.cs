using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class platform_fee_config
{
    public Guid config_id { get; set; }

    public FeeCode fee_code { get; set; }

    public string fee_name { get; set; } = null!;

    public FeeType? fee_type { get; set; }

    public Guid? target_host_id { get; set; }

    public CalcMethod calculation_method { get; set; }

    public decimal? percentage_value { get; set; }

    public decimal? fixed_amount { get; set; }

    public AppliedBaseOn? applied_base_on { get; set; }

    public int? priority { get; set; }

    public int? calculation_order { get; set; }

    public DateTime? effective_from { get; set; }

    public DateTime? effective_to { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual host_profile? target_host { get; set; }
}
