using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class Dispute
{
    public Guid DisputeId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? RaisedBy { get; set; }

    public string? DisputeType { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public Guid? AssignedModeratorId { get; set; }

    public string? ModeratorNote { get; set; }

    public string? ResolutionType { get; set; }

    public decimal? RefundAmount { get; set; }

    public Guid? AdminApprovalId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual User? AdminApproval { get; set; }

    public virtual User? AssignedModerator { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<DisputeAttachment> DisputeAttachments { get; set; } = new List<DisputeAttachment>();

    public virtual User? RaisedByNavigation { get; set; }
}
