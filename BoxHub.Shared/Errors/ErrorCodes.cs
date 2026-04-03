using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Shared.Errors
{
    public static class ErrorCodes
    {
        public const string Unauthorized = "UNAUTHORIZED";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string UsernameExists = "USERNAME_EXISTS";
        public const string EmailExists = "EMAIL_EXISTS";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string ServerError = "SERVER_ERROR";
        public const string UserInactive = "USER_INACTIVE";
        public const string OtpInvalid = "OTP_INVALID";
        public const string OtpRateLimited = "OTP_RATE_LIMITED";
        public const string DraftNotFound = "DRAFT_NOT_FOUND";
        public const string DraftLocked = "DRAFT_LOCKED";
        public const string TokenInvalid = "TOKEN_INVALID";
        public const string Forbidden = "FORBIDDEN";
        public const string EmailSendFailed = "EMAIL_SEND_FAILED";
        public const string HostProfileNotFound = "HOST_PROFILE_NOT_FOUND";
    }
}
