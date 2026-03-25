using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Facilities
{
    public class FacilitySearchItemResponse
    {
        public Guid FacilityId { get; set; }

        public string FacilityName { get; set; } = default!;

        public string? District { get; set; }

        public decimal? MinPrice { get; set; }

        public double AvgRating { get; set; }

        public int ReviewCount { get; set; }

        public List<string>? MatchedAmenities { get; set; }
    }
}
