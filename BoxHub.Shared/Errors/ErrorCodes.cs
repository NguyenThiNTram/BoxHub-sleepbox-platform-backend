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
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string ServerError = "SERVER_ERROR";
    }
}
