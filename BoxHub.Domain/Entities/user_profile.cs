using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class user_profile
{
    public Guid profile_id { get; set; }

    public Guid user_id { get; set; }

    public string? first_name { get; set; }

    public string? last_name { get; set; }

    public string? gender { get; set; }

    public DateOnly? date_of_birth { get; set; }

    public string? avatar_url { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual user user { get; set; } = null!;
}
