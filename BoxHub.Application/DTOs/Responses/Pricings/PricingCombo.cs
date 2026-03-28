using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class PricingCombo
    {
        public Guid ComboId { get; set; }

        public int Hours { get; set; }

        public decimal? ComboFactor { get; set; }

        public bool IsActive { get; set; }
    }
}
