using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? GuestId { get; set; }

    public Guid? BoxId { get; set; }

    public int? RatingScore { get; set; }

    public string? Comment { get; set; }

    public string? ReviewStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Sleepbox? Box { get; set; }

    public virtual User1? Guest { get; set; }
}
