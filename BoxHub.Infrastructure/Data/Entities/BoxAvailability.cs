using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class BoxAvailability
{
    public Guid AvailabilityId { get; set; }

    public Guid? BoxId { get; set; }

    public Guid? BookingId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Sleepbox? Box { get; set; }
}
