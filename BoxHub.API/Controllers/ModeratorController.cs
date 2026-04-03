using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Authorize(Roles = "MODERATOR")]
[Route("api")]
public sealed class ModeratorController : ControllerBase
{
    private readonly IHostRegistrationService _svc;

    public ModeratorController(IHostRegistrationService svc)
    {
        _svc = svc;
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
