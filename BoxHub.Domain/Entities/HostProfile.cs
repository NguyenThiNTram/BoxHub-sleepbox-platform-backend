using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class HostProfile
{
    public Guid HostId { get; set; }

    public Guid? UserId { get; set; }

    public string? BrandName { get; set; }

    public string? RepresentativeName { get; set; }

    public string? RepresentativeIdNumber { get; set; }

    public string? TaxCode { get; set; }

    public string? BusinessAddress { get; set; }

    public string? BusinessWard { get; set; }

    public string? BusinessDistrict { get; set; }

    public string? BusinessCity { get; set; }

    public string? VerifiedStatus { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<HostDocument> HostDocuments { get; set; } = new List<HostDocument>();

    public virtual ICollection<HostPayout> HostPayouts { get; set; } = new List<HostPayout>();

    public virtual User? User { get; set; }
}
