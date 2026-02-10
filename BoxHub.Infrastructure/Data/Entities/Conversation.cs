using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Conversation
{
    public Guid ConversationId { get; set; }

    public Guid? GuestId { get; set; }

    public Guid? HostId { get; set; }

    public Guid? StaffId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? FacilityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Facility? Facility { get; set; }

    public virtual User1? Guest { get; set; }

    public virtual HostProfile? Host { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual StaffProfile? Staff { get; set; }
}
