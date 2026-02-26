using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class AddonService
{
    public Guid ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? Unit { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BookingAddonItem> BookingAddonItems { get; set; } = new List<BookingAddonItem>();

    public virtual ICollection<HostAddonPrice> HostAddonPrices { get; set; } = new List<HostAddonPrice>();
}
