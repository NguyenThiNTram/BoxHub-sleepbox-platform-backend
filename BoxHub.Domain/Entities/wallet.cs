using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class wallet
{
    public Guid wallet_id { get; set; }

    public string wallet_type { get; set; } = null!;

    public Guid? host_id { get; set; }

    public string? currency { get; set; }

    public decimal? available_balance { get; set; }

    public decimal? pending_balance { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual host_profile? host { get; set; }

    public virtual ICollection<wallet_transaction> wallet_transactions { get; set; } = new List<wallet_transaction>();

    public virtual ICollection<withdrawal_request> withdrawal_requests { get; set; } = new List<withdrawal_request>();
}
