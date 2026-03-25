using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class booking_box
{
    public Guid booking_box_id { get; set; }

    public Guid booking_id { get; set; }

    public Guid box_id { get; set; }

    public decimal? box_price { get; set; }

    public string? box_name_snapshot { get; set; }

    public string? box_type_snapshot { get; set; }

    public DateTime? created_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual sleepbox box { get; set; } = null!;
}
