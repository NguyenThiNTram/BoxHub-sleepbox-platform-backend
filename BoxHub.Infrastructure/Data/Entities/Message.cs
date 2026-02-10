using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class Message
{
    public Guid MessageId { get; set; }

    public Guid? ConversationId { get; set; }

    public Guid? SenderId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt { get; set; }

    public bool? IsRead { get; set; }

    public virtual Conversation? Conversation { get; set; }

    public virtual User1? Sender { get; set; }
}
