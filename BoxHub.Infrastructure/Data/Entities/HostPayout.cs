using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class HostPayout
{
    public Guid PayoutId { get; set; }

    public Guid? HostId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? PaymentId { get; set; }

    public Guid? CommissionId { get; set; }

    public decimal? GrossAmount { get; set; }

    public decimal? CommissionAmount { get; set; }

    public decimal? NetAmount { get; set; }

    public string? PayoutStatus { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Commission? Commission { get; set; }

    public virtual HostProfile? Host { get; set; }

    public virtual Payment? Payment { get; set; }
}
