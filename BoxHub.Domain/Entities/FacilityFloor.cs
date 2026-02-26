using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class FacilityFloor
{
    public Guid FloorId { get; set; }

    public Guid? FacilityId { get; set; }

    public string? FloorName { get; set; }

    public string? Description { get; set; }

    public string? FloorAmenities { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual ICollection<Sleepbox> Sleepboxes { get; set; } = new List<Sleepbox>();
}
