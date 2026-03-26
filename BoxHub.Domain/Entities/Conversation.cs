using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class conversation
{
    public Guid conversation_id { get; set; }

    public string conversation_type { get; set; } = null!;

    public Guid? guest_id { get; set; }

    public Guid? host_id { get; set; }

    public Guid? staff_id { get; set; }

    public Guid? booking_id { get; set; }

    public Guid? facility_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? last_message_at { get; set; }

    public virtual booking? booking { get; set; }

    public virtual facility? facility { get; set; }

    public virtual user? guest { get; set; }

    public virtual host_profile? host { get; set; }

    public virtual ICollection<message> messages { get; set; } = new List<message>();

    public virtual staff_profile? staff { get; set; }
}
