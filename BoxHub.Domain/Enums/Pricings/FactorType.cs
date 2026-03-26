using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Enums.Pricings
{
    public enum FactorType
    {
        LOCATION,
        TIME_SLOT, // E.g., peak hours, off-peak hours
        DAY // E.g., weekday, weekend, holiday
    }
}
