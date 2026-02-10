using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class HostDocument
{
    public Guid DocumentId { get; set; }

    public Guid? HostId { get; set; }

    public string? DocumentType { get; set; }

    public string? FileUrl { get; set; }

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? DocumentStatus { get; set; }

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectReason { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual HostProfile? Host { get; set; }

    public virtual User1? ReviewedByNavigation { get; set; }
}
