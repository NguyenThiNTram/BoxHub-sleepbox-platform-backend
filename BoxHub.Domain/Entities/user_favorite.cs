using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class user_favorite
{
    public Guid favorite_id { get; set; }

    public Guid user_id { get; set; }

    public Guid facility_id { get; set; }

    public virtual facility facility { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
