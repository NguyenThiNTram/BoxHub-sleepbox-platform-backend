using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Moderators
{
    public class HostInfo
    {
        public Guid HostId { get; set; }

        public string? RepresentativeName { get; set; }
        public string? RepresentativeIdNumber { get; set; }

        public string? TaxCode { get; set; }

        public string? BusinessName { get; set; }

        public string? AddressDistrict { get; set; }
        public string? AddressWard { get; set; }
        public string? AddressDetail { get; set; }

        public string? RepresentativeFrontUrl { get; set; }
        public string? RepresentativeBackUrl { get; set; }

        public string VerifiedStatus { get; set; } = string.Empty;

        public DateTime? SubmittedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? RejectReason { get; set; }
    }
}
