using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class DisputeAttachment
{
    public Guid AttachmentId { get; set; }

    public Guid? DisputeId { get; set; }

    public string? AttachmentRole { get; set; }

    public string? FileUrl { get; set; }

    public string? FileType { get; set; }

    public Guid? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Dispute? Dispute { get; set; }

    public virtual User? UploadedByNavigation { get; set; }
}
