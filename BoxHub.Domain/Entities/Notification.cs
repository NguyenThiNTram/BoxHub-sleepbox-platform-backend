using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? NotificationType { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
