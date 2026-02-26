using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class PaymentTransaction
{
    public Guid TransactionId { get; set; }

    public Guid? PaymentId { get; set; }

    public string? Provider { get; set; }

    public string? Environment { get; set; }

    public string? GatewayTransactionId { get; set; }

    public string? RequestType { get; set; }

    public string? TransactionStatus { get; set; }

    public string? CallbackPayload { get; set; }

    public DateTime? CallbackReceivedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Payment? Payment { get; set; }
}
