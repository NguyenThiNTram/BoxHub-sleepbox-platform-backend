using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public string? BookingCode { get; set; }

    public Guid? GuestId { get; set; }

    public Guid? BoxId { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public DateTime? ActualCheckIn { get; set; }

    public DateTime? ActualCheckOut { get; set; }

    public decimal? TotalBoxPrice { get; set; }

    public decimal? TotalPlatformFee { get; set; }

    public decimal? TotalAmenityPrice { get; set; }

    public decimal? FinalAmount { get; set; }

    public string? BookingStatus { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledById { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookingAddonItem> BookingAddonItems { get; set; } = new List<BookingAddonItem>();

    public virtual Sleepbox? Box { get; set; }

    public virtual ICollection<BoxAvailability> BoxAvailabilities { get; set; } = new List<BoxAvailability>();

    public virtual User1? CancelledBy { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    public virtual User1? Guest { get; set; }

    public virtual ICollection<HostPayout> HostPayouts { get; set; } = new List<HostPayout>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Review? Review { get; set; }
}
