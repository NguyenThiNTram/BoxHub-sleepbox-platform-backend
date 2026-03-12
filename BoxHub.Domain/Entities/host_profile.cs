using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class host_profile
{
    public Guid host_id { get; set; }

    public Guid user_id { get; set; }

    public string? representative_name { get; set; }

    public string? representative_id_number { get; set; }

    public string? tax_code { get; set; }

    public string? business_address { get; set; }

    public string? verified_status { get; set; }

    public DateTime? verified_at { get; set; }

    public Guid? verified_by { get; set; }

    public string? verified_note { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking> bookings { get; set; } = new List<booking>();

    public virtual ICollection<brand> brands { get; set; } = new List<brand>();

    public virtual ICollection<conversation> conversations { get; set; } = new List<conversation>();

    public virtual ICollection<host_document> host_documents { get; set; } = new List<host_document>();

    public virtual host_payout_account? host_payout_account { get; set; }

    public virtual ICollection<platform_fee_config> platform_fee_configs { get; set; } = new List<platform_fee_config>();

    public virtual user user { get; set; } = null!;

    public virtual user? verified_byNavigation { get; set; }

    public virtual ICollection<wallet> wallets { get; set; } = new List<wallet>();

    public virtual ICollection<withdrawal_request> withdrawal_requests { get; set; } = new List<withdrawal_request>();
}
