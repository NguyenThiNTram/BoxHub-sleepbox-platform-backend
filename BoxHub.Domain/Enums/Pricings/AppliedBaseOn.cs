using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Enums.Pricings
{
    public enum AppliedBaseOn
    {
        BOX_PRICE,          // chỉ giá box (KHÔNG gồm addon) dùng cho COMMISSION
        SUBTOTAL,           // box + addon
        TOTAL_BOOKING,      // subtotal + service fee
        COMMISSION_AMOUNT   // dùng cho TAX tính trên commission
    }
}
