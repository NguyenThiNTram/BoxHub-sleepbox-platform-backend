using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Pricings
{
    public class PriceRuleDetail : PriceRuleSummary
    {
        // for get by id, include factors/combo details
        public List<PricingFactor> Factors { get; set; } = new();

        public List<PricingCombo> Combos { get; set; } = new();
    }
}
