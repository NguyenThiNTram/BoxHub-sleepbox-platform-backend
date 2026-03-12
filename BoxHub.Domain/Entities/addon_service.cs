using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class addon_service
{
    public Guid service_id { get; set; }

    public string service_name { get; set; } = null!;

    public string? unit { get; set; }

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual ICollection<booking_addon_service> booking_addon_services { get; set; } = new List<booking_addon_service>();

    public virtual ICollection<host_addon_price> host_addon_prices { get; set; } = new List<host_addon_price>();
}
