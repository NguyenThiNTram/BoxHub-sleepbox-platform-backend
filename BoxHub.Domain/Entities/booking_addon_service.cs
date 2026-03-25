using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class booking_addon_service
{
    public Guid booking_addon_id { get; set; }

    public Guid booking_id { get; set; }

    public Guid? service_id { get; set; }

    public string? service_name { get; set; }

    public decimal? unit_price { get; set; }

    public decimal? quantity { get; set; }

    public DateTime? created_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual addon_service? service { get; set; }
}
