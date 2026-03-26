using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_payout_account
{
    public Guid account_id { get; set; }

    public Guid host_id { get; set; }

    public string payment_method { get; set; } = null!;

    public string? account_name { get; set; }

    public string account_number { get; set; } = null!;

    public string? bank_name { get; set; }

    public string? bank_branch { get; set; }

    public bool? is_primary { get; set; }

    public DateTime created_at { get; set; }

    public virtual host_profile host { get; set; } = null!;

    public virtual ICollection<withdrawal_request> withdrawal_requests { get; set; } = new List<withdrawal_request>();
}
