using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Sleepbox
{
    public Guid BoxId { get; set; }

    public Guid? FloorId { get; set; }

    public string? BoxName { get; set; }

    public string? BoxType { get; set; }

    public string? BoxRow { get; set; }

    public string? BoxLevel { get; set; }

    public decimal? SizeWidth { get; set; }

    public decimal? SizeLength { get; set; }

    public decimal? SizeHeight { get; set; }

    public int? CleaningBufferMinutes { get; set; }

    public string? BoxStatus { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<BoxAvailability> BoxAvailabilities { get; set; } = new List<BoxAvailability>();

    public virtual FacilityFloor? Floor { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
}
