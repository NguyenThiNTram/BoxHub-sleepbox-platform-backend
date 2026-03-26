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
        // Hỗ trợ cả header `token` (BE hiện tại) và `Authorization: Bearer <token>` (Swagger Authorize).
        string? token = null;

        if (Request.Headers.TryGetValue("token", out var tokenHeader))
            token = tokenHeader.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token) &&
            Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var value = authHeader.ToString().Trim();
            // Hỗ trợ cả "Bearer <token>" và "<token>".
            if (value.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                var rest = value.Substring("Bearer".Length).Trim();
                token = rest;
            }
            else
            {
                token = value;
            }
        }

        var result = await _svc.SetPasswordAsync(token, request, ct);
        if (!result.IsSuccess)
            return Problem(
                title: result.ErrorCode ?? ErrorCodes.ValidationFailed,
                detail: result.ErrorMessage,
                statusCode: result.HttpStatus ?? 400);

        return Ok(result.Value);
    }
}
