using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/host/payout-account")]
[Authorize(Roles = "HOST")]
public sealed class HostPayoutController : ControllerBase
{
    private readonly IHostPayoutService _payouts;

    public HostPayoutController(IHostPayoutService payouts)
    {
        _payouts = payouts;
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        var result = await _payouts.GetPayoutForCurrentHostAsync(userId.Value, ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult> Update(
        [FromBody] UpdateHostPayoutAccountRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        var result = await _payouts.UpdatePayoutForCurrentHostAsync(userId.Value, request, ct);
        return Ok(result);
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
