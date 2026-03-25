using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class staff_profile
{
    public Guid staff_id { get; set; }

    public Guid user_id { get; set; }

    public Guid facility_id { get; set; }

    public string? position { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<conversation> conversations { get; set; } = new List<conversation>();

    public virtual facility facility { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
