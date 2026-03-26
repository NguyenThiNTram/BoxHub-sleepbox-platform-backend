using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class booking
{
    public Guid booking_id { get; set; }

    public string? booking_code { get; set; }

    public Guid? guest_id { get; set; }

    public Guid? facility_id { get; set; }

    public Guid? host_id { get; set; }

    public DateTime? check_in { get; set; }

    public DateTime? check_out { get; set; }

    public DateTime? actual_check_in { get; set; }

    public DateTime? actual_check_out { get; set; }

    public string? pricing_snapshot { get; set; }

    public decimal? total_box_price { get; set; }

    public decimal? total_addon_item_price { get; set; }

    public decimal? commission_amount { get; set; }

    public decimal? service_fee { get; set; }

    public decimal? vat_amount { get; set; }

    public decimal? final_amount { get; set; }

    public string? booking_status { get; set; }

    public string? payment_status { get; set; }

    public DateTime? cancelled_at { get; set; }

    public Guid? cancelled_by_id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking_addon_service> booking_addon_services { get; set; } = new List<booking_addon_service>();

    public virtual ICollection<booking_box> booking_boxes { get; set; } = new List<booking_box>();

    public virtual ICollection<booking_status_history> booking_status_histories { get; set; } = new List<booking_status_history>();

    public virtual ICollection<box_availability> box_availabilities { get; set; } = new List<box_availability>();

    public virtual user? cancelled_by { get; set; }

    public virtual ICollection<conversation> conversations { get; set; } = new List<conversation>();

    public virtual ICollection<dispute> disputes { get; set; } = new List<dispute>();

    public virtual facility? facility { get; set; }

    public virtual user? guest { get; set; }

    public virtual host_profile? host { get; set; }

    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    public virtual review? review { get; set; }
}
