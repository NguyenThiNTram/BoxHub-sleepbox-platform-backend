using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Shared.Errors
{
    public static class ErrorFactory
    {
        public static ApiError Unauthorized(HttpContext ctx) =>
            ApiError.Create(
                ErrorCodes.Unauthorized,
                "Unauthorized",
                StatusCodes.Status401Unauthorized,
                ctx.TraceIdentifier
            );

        public static ApiError UserNotFound(HttpContext ctx) =>
            ApiError.Create(
                ErrorCodes.UserNotFound,
                "User not found",
                StatusCodes.Status404NotFound,
                ctx.TraceIdentifier
            );

        public static ApiError UsernameExists(HttpContext ctx) =>
            ApiError.Create(
                ErrorCodes.UsernameExists,
                "Username already exists",
                StatusCodes.Status409Conflict,
                ctx.TraceIdentifier
            );

        public static ApiError Validation(HttpContext ctx, object details) =>
            ApiError.Create(
                ErrorCodes.ValidationFailed,
                "Validation failed",
                StatusCodes.Status400BadRequest,
                ctx.TraceIdentifier,
                details
            );

        public static ApiError Server(HttpContext ctx) =>
            ApiError.Create(
                ErrorCodes.ServerError,
                "Internal server error",
                StatusCodes.Status500InternalServerError,
                ctx.TraceIdentifier
            );
    }
}
