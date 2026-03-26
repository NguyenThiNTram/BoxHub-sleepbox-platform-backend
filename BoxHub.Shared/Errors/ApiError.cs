using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Shared.Errors
{
    public class ApiError
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public int Status { get; set; }
        public object? Details { get; set; }

        public static ApiError Create(string code, string message, int status, string? traceId = null, object? details = null)
        {
            return new ApiError
            {
                Code = code,
                Message = message,
                Status = status,
                Details = details
            };
        }
    }
}
