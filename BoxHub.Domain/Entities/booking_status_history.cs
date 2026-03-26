using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class booking_status_history
{
    public Guid history_id { get; set; }

    public Guid booking_id { get; set; }

    public Guid? payment_id { get; set; }

    public string? old_status { get; set; }

    public string? new_status { get; set; }

    public DateTime? old_check_in { get; set; }

    public DateTime? old_check_out { get; set; }

    public DateTime? new_check_in { get; set; }

    public DateTime? new_check_out { get; set; }

    public string? note { get; set; }

    public DateTime changed_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual payment? payment { get; set; }
}
