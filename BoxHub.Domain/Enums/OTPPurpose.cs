using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Enums
{
    public enum OTPPurpose
    {
        VERIFY_EMAIL,
        FORGOT_PASSWORD,
        CHANGE_EMAIL,
        RESET_PASSWORD,
        HOST_REGISTER,
    }
}
