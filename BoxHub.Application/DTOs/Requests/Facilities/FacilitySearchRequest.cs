using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Facilities
{
    public class FacilitySearchRequest : PagingRequest
    {
        // Search
        public string? Search { get; set; }

        //Khu vực
        public List<string>? Areas { get; set; }

        //Rating (4-5 sao)
        public int? MinRating { get; set; }

        // Giá
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        // Room type
        public string? BoxType { get; set; }

        // Availability
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        // Amenities
        public List<int>? AmenityIds { get; set; }
    }
}
