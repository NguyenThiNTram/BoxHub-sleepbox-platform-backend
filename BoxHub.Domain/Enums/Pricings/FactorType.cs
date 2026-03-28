using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Enums.Pricings
{
    public enum FactorType
    {
        LOCATION = 1,
        TIME_SLOT = 2, // E.g., peak hours, off-peak hours
        DAY = 3, // E.g., weekday, weekend, holiday
        OVERNIGHT = 4 // Specific to overnight pricing
    }
}
