using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Shared.Errors
{
    public class ApiException : Exception
    {
        public string Code { get; }
        public int Status { get; }

        public ApiException(string code, string message, int status) : base(message)
        {
            Code = code;
            Status = status;
        }
    }
}
