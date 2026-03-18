using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoxHub.API.Controllers
{
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateAdmin(
        [FromBody] CreateAdminRequest request,
        CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorFactory.Validation(HttpContext, ModelState));

            var userId = GetCurrentUserId();

            var result = await _adminService.CreateAdminAsync(userId, request, ct);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ApiException(
                    ErrorCodes.Unauthorized, "Invalid or missing token",
                    StatusCodes.Status401Unauthorized);
            }

            return userId;
        }
    }
}
