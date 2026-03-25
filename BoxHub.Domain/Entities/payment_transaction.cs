using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class payment_transaction
{
    public Guid transaction_id { get; set; }

    public Guid payment_id { get; set; }

    public string provider { get; set; } = null!;

    public string? environment { get; set; }

    public string? gateway_transaction_id { get; set; }

    public string? request_type { get; set; }

    public string? transaction_status { get; set; }

    public string? provider_response_code { get; set; }

    public string? callback_payload { get; set; }

    public DateTime? callback_received_at { get; set; }

    public DateTime created_at { get; set; }

    public virtual payment payment { get; set; } = null!;
}
