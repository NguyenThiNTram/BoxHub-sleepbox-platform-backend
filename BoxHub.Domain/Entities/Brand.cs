using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class brand
{
    public Guid brand_id { get; set; }

    public Guid host_id { get; set; }

    public string brand_name { get; set; } = null!;

    public string? brand_avatar { get; set; }

    public DateTime? created_at { get; set; } = DateTime.UtcNow;

    public DateTime? updated_at { get; set; }

    public BrandStatus status { get; set; } = BrandStatus.Pending;

    public bool is_deleted { get; set; } = false;

    public DateTime? deleted_at { get; set; }

    public virtual ICollection<facility> facilities { get; set; } = new List<facility>();

    public virtual host_profile host { get; set; } = null!;
}
