using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/host")]
public sealed class HostPasswordController : ControllerBase
{
    private readonly IHostRegistrationService _svc;

    public HostPasswordController(IHostRegistrationService svc)
    {
        _svc = svc;
    }

    /// <summary>POST /api/host/set-password — header token (JWT đặt mật khẩu).</summary>
    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] HostSetPasswordRequest request, CancellationToken ct)
    {
        Request.Headers.TryGetValue("token", out var tokenHeader);
        var token = tokenHeader.FirstOrDefault();
        var result = await _svc.SetPasswordAsync(token, request, ct);
        if (!result.IsSuccess)
            return Problem(
                title: result.ErrorCode ?? ErrorCodes.ValidationFailed,
                detail: result.ErrorMessage,
                statusCode: result.HttpStatus ?? 400);

        return Ok(result.Value);
    }
}
