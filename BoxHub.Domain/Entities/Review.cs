using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class review
{
    public Guid review_id { get; set; }

    public Guid booking_id { get; set; }

    public Guid guest_id { get; set; }

    public Guid facility_id { get; set; }

    public int rating_score { get; set; }

    public string? comment { get; set; }

    public DateTime created_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual facility facility { get; set; } = null!;

    public virtual user guest { get; set; } = null!;
}
