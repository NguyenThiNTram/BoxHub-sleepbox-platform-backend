using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class Facility
{
    public Guid FacilityId { get; set; }

    public Guid? BrandId { get; set; }

    public string? FacilityName { get; set; }

    public string? Description { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressWard { get; set; }

    public string? AddressDistrict { get; set; }

    public string? AddressCity { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? HouseRules { get; set; }

    public string? FacilityStatus { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<FacilityFloor> FacilityFloors { get; set; } = new List<FacilityFloor>();

    public virtual ICollection<HostAddonPrice> HostAddonPrices { get; set; } = new List<HostAddonPrice>();

    public virtual ICollection<HostBasePrice> HostBasePrices { get; set; } = new List<HostBasePrice>();

    public virtual ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();

    public virtual ICollection<StaffProfile> StaffProfiles { get; set; } = new List<StaffProfile>();
}
