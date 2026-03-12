using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class withdrawal_request
{
    public Guid request_id { get; set; }

    public string? request_code { get; set; }

    public Guid host_id { get; set; }

    public Guid wallet_id { get; set; }

    public Guid account_id { get; set; }

    public decimal amount { get; set; }

    public string? status { get; set; }

    public string? bank_transaction_code { get; set; }

    public string? admin_note { get; set; }

    public DateTime requested_at { get; set; }

    public DateTime? processed_at { get; set; }

    public virtual host_payout_account account { get; set; } = null!;

    public virtual host_profile host { get; set; } = null!;

    public virtual wallet wallet { get; set; } = null!;
}
