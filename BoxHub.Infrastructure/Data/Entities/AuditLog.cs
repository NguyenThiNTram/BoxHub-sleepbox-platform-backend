using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class AuditLog
{
    public Guid AuditId { get; set; }

    public Guid? ActorId { get; set; }

    public string? ActorRole { get; set; }

    public string? Action { get; set; }

    public string? TargetType { get; set; }

    public Guid? TargetId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User1? Actor { get; set; }
}
