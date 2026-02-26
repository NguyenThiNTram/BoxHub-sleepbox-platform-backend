using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class MediaAsset
{
    public Guid MediaId { get; set; }

    public Guid? TargetId { get; set; }

    public string? MediaType { get; set; }

    public string? MediaUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsCover { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Facility? Target { get; set; }
}
