using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class wallet_transaction
{
    public Guid transaction_id { get; set; }

    public Guid wallet_id { get; set; }

    public string transaction_type { get; set; } = null!;

    public string? balance_type { get; set; }

    public string direction { get; set; } = null!;

    public decimal amount { get; set; }

    public string? reference_type { get; set; }

    public Guid? reference_id { get; set; }

    public decimal? available_balance_after { get; set; }

    public decimal? pending_balance_after { get; set; }

    public string? transaction_status { get; set; }

    public string? description { get; set; }

    public DateTime created_at { get; set; }

    public virtual wallet wallet { get; set; } = null!;
}
