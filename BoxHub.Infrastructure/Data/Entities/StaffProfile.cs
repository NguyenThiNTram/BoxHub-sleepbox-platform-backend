using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class StaffProfile
{
    public Guid StaffId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? FacilityId { get; set; }

    public string? Position { get; set; }

    public string? WorkplaceNote { get; set; }

    public string? JobDescription { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual Facility? Facility { get; set; }

    public virtual User1? User { get; set; }
}
