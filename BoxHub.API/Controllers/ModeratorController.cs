using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BoxHub.Application.DTOs.Requests.Hosts;
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
        private readonly IHostRegistrationService _svc;

        public ModeratorController(IModeratorService moderatorService, IHostRegistrationService svc)
        {
                _moderatorService = moderatorService;
                _svc = svc;
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

        [HttpGet("host-drafts")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null, CancellationToken ct = default)
        {
            var result = await _svc.ListDraftsForModeratorAsync(page, pageSize, status, ct);
            if (!result.IsSuccess)
                return Problem(
                    title: result.ErrorCode,
                    detail: result.ErrorMessage,
                    statusCode: result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpGet("host-draft/{draftId:guid}")]
        public async Task<IActionResult> Detail([FromRoute] Guid draftId, CancellationToken ct)
        {
            var result = await _svc.GetDraftDetailForModeratorAsync(draftId, ct);
            if (!result.IsSuccess)
                return Problem(
                    title: result.ErrorCode,
                    detail: result.ErrorMessage,
                    statusCode: result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        [HttpPost("host-draft/{draftId:guid}/review")]
        public async Task<IActionResult> Review(
            [FromRoute] Guid draftId,
            [FromBody] ModeratorReviewHostDraftRequest request,
            CancellationToken ct)
        {
            var modId = GetModeratorUserId();
            if (modId == null)
                return Unauthorized();

            var result = await _svc.ModeratorReviewAsync(draftId, modId.Value, request, ct);
            if (!result.IsSuccess)
                return Problem(
                    title: result.ErrorCode,
                    detail: result.ErrorMessage,
                    statusCode: result.HttpStatus ?? 400);

            return Ok(result.Value);
        }

        private Guid? GetModeratorUserId()
        {
            var sub =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(sub, out var id) ? id : null;
        }

    }
}
