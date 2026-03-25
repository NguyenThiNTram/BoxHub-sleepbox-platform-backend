using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class box_availability
{
    public Guid availability_id { get; set; }

    public Guid facility_id { get; set; }

    public Guid box_id { get; set; }

    public Guid? booking_id { get; set; }

    public DateTime start_time { get; set; }

    public DateTime end_time { get; set; }

    public string? availability_status { get; set; }

    public DateTime? locked_until { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual booking? booking { get; set; }

    public virtual sleepbox box { get; set; } = null!;

    public virtual facility facility { get; set; } = null!;
}
