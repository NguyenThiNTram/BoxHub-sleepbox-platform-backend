using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class sleepbox
{
    public Guid box_id { get; set; }

    public Guid area_id { get; set; }

    public string box_name { get; set; } = null!;

    public string? box_type { get; set; }

    public string? box_class { get; set; }

    public int? row_number { get; set; }

    public int? level_number { get; set; }

    public decimal? size_width { get; set; }

    public decimal? size_length { get; set; }

    public decimal? size_height { get; set; }

    public string? box_status { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual facility_area area { get; set; } = null!;

    public virtual ICollection<booking_box> booking_boxes { get; set; } = new List<booking_box>();

    public virtual ICollection<box_availability> box_availabilities { get; set; } = new List<box_availability>();

    //public virtual ICollection<amenity> amenities { get; set; } = new List<amenity>();

    public ICollection<sleepbox_amenity> sleepbox_amenities { get; set; }
}
