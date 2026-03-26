using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Shared.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public List<string>? Warnings { get; set; }
        public int StatusCode { get; set; }

        // SUCCESS
        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        // FAILURE
        public static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }

        // ADD WARNING
        public void AddWarning(string warning)
        {
            if (Warnings == null)
                Warnings = new List<string>();

            Warnings.Add(warning);
        }
    }
}
