using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_addon_price
{
    public Guid host_service_id { get; set; }

    public Guid facility_id { get; set; }

    public Guid service_id { get; set; }

    public decimal price { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual facility facility { get; set; } = null!;

    public virtual addon_service service { get; set; } = null!;
}
