using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api")]
public sealed class OTPController : ControllerBase
{
    private readonly IHostRegistrationService _svc;

    public OTPController(IHostRegistrationService svc)
    {
        _svc = svc;
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<VerifyOtpResponse>> Verify([FromBody] VerifyOtpRequest request, CancellationToken ct)
    {
        var result = await _svc.VerifyOtpAsync(request, ct);
        if (!result.IsSuccess)
            return StatusCode(
                result.HttpStatus ?? 400,
                new VerifyOtpResponse { Success = false, Message = result.ErrorMessage ?? "" });

        return Ok(result.Value);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> Resend([FromBody] ResendOtpRequest request, CancellationToken ct)
    {
        var result = await _svc.ResendOtpAsync(request, ct);
        if (!result.IsSuccess)
            return Problem(
                title: result.ErrorCode ?? ErrorCodes.ValidationFailed,
                detail: result.ErrorMessage,
                statusCode: result.HttpStatus ?? 400);

        return Ok(result.Value);
    }
}
