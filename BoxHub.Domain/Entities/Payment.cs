using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? BookingId { get; set; }

    public string? ClientRequestId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentType { get; set; }

    public string? PaymentStatus { get; set; }

    public string? ReferenceNote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<HostPayout> HostPayouts { get; set; } = new List<HostPayout>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
