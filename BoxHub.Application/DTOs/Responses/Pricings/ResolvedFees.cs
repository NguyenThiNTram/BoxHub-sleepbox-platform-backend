using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class ResolvedFees
    {
        public decimal Commission { get; set; }

        public decimal ServiceFee { get; set; }

        public decimal Vat { get; set; }
    }
}
