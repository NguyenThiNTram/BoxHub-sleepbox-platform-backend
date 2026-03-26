using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Requests.Pricings
{
    public class UpdateBoxTypePriceLimitRequest
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
