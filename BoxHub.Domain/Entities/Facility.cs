using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class facility
{
    public Guid facility_id { get; set; }

    public Guid brand_id { get; set; }

    public string facility_name { get; set; } = null!;

    public string? description { get; set; }

    public string? address_street { get; set; }

    public string? address_ward { get; set; }

    public string? address_district { get; set; }

    public string? address_city { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public string? house_rules { get; set; }

    public int? cleaning_buffer_minutes { get; set; }

    public string? facility_status { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking> bookings { get; set; } = new List<booking>();

    public virtual ICollection<box_availability> box_availabilities { get; set; } = new List<box_availability>();

    public virtual brand brand { get; set; } = null!;

    public virtual ICollection<conversation> conversations { get; set; } = new List<conversation>();

    public virtual ICollection<facility_area> facility_areas { get; set; } = new List<facility_area>();

    public virtual ICollection<host_addon_price> host_addon_prices { get; set; } = new List<host_addon_price>();

    public virtual ICollection<host_base_price> host_base_prices { get; set; } = new List<host_base_price>();

    public virtual ICollection<review> reviews { get; set; } = new List<review>();

    public virtual ICollection<staff_profile> staff_profiles { get; set; } = new List<staff_profile>();

    public virtual ICollection<user_favorite> user_favorites { get; set; } = new List<user_favorite>();

    //public virtual ICollection<amenity> amenities { get; set; } = new List<amenity>();

    public ICollection<facility_amenity> facility_amenities { get; set; }

    public virtual ICollection<facility_document> facility_documents { get; set; } = new List<facility_document>();
}
