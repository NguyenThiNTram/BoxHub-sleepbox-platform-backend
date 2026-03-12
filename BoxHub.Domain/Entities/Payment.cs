using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class payment
{
    public Guid payment_id { get; set; }

    public Guid booking_id { get; set; }

    public string? client_request_id { get; set; }

    public decimal amount { get; set; }

    public string? currency { get; set; }

    public string? payment_type { get; set; }

    public string? payment_status { get; set; }

    public string? reference_note { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual ICollection<booking_status_history> booking_status_histories { get; set; } = new List<booking_status_history>();

    public virtual ICollection<payment_transaction> payment_transactions { get; set; } = new List<payment_transaction>();
}
