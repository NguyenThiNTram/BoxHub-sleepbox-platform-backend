using BoxHub.Application.DTOs.Requests.Moderators;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Moderators;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoxHub.API.Controllers
{
    [ApiController]
    [Route("api/moderator/users")]
    [Authorize(Roles = "MODERATOR")]
    public class ModeratorController : ControllerBase
    {
        private readonly IModeratorService _moderatorService;

        public ModeratorController(IModeratorService moderatorService)
        {
            _moderatorService = moderatorService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<AccountListItem>>> GetUsers(
            [FromQuery] AccountFilter filter,
            CancellationToken ct)
        {
            var result = await _moderatorService.GetUsersAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<AccountDetail>> GetUserDetail(
            Guid userId,
            CancellationToken ct)
        {
            var result = await _moderatorService.GetUserDetailAsync(userId, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{userId:guid}/suspend")]
        public async Task<ActionResult<SuspendAccountResult>> SuspendUser(
            Guid userId,
            [FromBody] SuspendAccountRequest request,
            CancellationToken ct)
        {
            // get actor info
            var actorId = GetUserId();
            var actorRole = GetUserRole();

            var result = await _moderatorService.SuspendUserAsync(
                userId,
                request,
                actorId,
                actorRole,
                ct);

            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return Guid.Parse(userId);
        }

        private UserRole GetUserRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Invalid role");

            return Enum.Parse<UserRole>(role, true);
        }
    }
}