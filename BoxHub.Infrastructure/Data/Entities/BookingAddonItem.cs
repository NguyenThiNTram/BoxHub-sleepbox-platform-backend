using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class BookingAddonItem
{
    public Guid BookingAddonId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? ServiceId { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual AddonService? Service { get; set; }
}
