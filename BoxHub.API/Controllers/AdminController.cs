using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoxHub.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ProblemResult(ErrorCodes.ValidationFailed, "Validation failed", 400);

            var userIdResult = GetCurrentUserId();
            if (!userIdResult.IsSuccess)
                return ProblemResult(userIdResult.ErrorCode, userIdResult.ErrorMessage, userIdResult.HttpStatus ?? 401);

            var result = await _adminService.CreateAdminAsync(userIdResult.Value, request, ct);
            if (!result.IsSuccess)
                return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpPatch("users/{targetUserId}/suspend")]
        public async Task<IActionResult> SuspendUser([FromRoute] Guid targetUserId, CancellationToken ct)
        {
            var userIdResult = GetCurrentUserId();
            if (!userIdResult.IsSuccess)
                return ProblemResult(userIdResult.ErrorCode, userIdResult.ErrorMessage, userIdResult.HttpStatus ?? 401);

            var result = await _adminService.SuspendUserAsync(userIdResult.Value, targetUserId, ct);
            if (!result.IsSuccess)
                return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpPost("moderators/register")]
        public async Task<IActionResult> CreateModerator([FromBody] CreateModeratorRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ProblemResult(ErrorCodes.ValidationFailed, "Validation failed", 400);

            var userIdResult = GetCurrentUserId();
            if (!userIdResult.IsSuccess)
                return ProblemResult(userIdResult.ErrorCode, userIdResult.ErrorMessage, userIdResult.HttpStatus ?? 401);

            var result = await _adminService.CreateModeratorAsync(userIdResult.Value, request, ct);
            if (!result.IsSuccess)
                return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpPut("moderators/{moderatorId}")]
        public async Task<IActionResult> UpdateModerator(Guid moderatorId, [FromBody] UpdateModeratorRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ProblemResult(ErrorCodes.ValidationFailed, "Validation failed", 400);

            var userIdResult = GetCurrentUserId();
            if (!userIdResult.IsSuccess)
                return ProblemResult(userIdResult.ErrorCode, userIdResult.ErrorMessage, userIdResult.HttpStatus ?? 401);

            var result = await _adminService.UpdateModeratorAsync(userIdResult.Value, moderatorId, request, ct);
            if (!result.IsSuccess)
                return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpGet("users/get_list")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] GetUsersRequest request,
            CancellationToken ct)
        {
            var userIdResult = GetCurrentUserId();
            if (!userIdResult.IsSuccess)
                return ProblemResult(userIdResult.ErrorCode, userIdResult.ErrorMessage, userIdResult.HttpStatus ?? 401);

            var result = await _adminService.GetUsersAsync(userIdResult.Value, request, ct);
            if (!result.IsSuccess)
                return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        private Result<Guid> GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Result<Guid>.Failure(
                    ErrorCodes.Unauthorized,
                    "Invalid or missing token",
                    401
                );
            }

            return Result<Guid>.Success(userId);
        }

        private static ObjectResult ProblemResult(string? code, string? message, int status) =>
            new(new ProblemDetails
            {
                Title = code ?? ErrorCodes.ValidationFailed,
                Detail = message,
                Status = status
            })
            {
                StatusCode = status
            };
    }
}