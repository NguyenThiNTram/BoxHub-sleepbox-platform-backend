using BoxHub.Application.DTOs.Requests.Otp;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/otp")]
public sealed class OTPController : ControllerBase
{
    private readonly IOtpService _otpService;

    public OTPController(IOtpService svc)
    {
        _otpService = svc;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendOtpRequest request, CancellationToken ct)
    {
        var result = await _otpService.SendAsync(request, ct);
        if (!result.IsSuccess)
            return Problem(
                title: result.ErrorCode ?? ErrorCodes.ValidationFailed,
                detail: result.ErrorMessage,
                statusCode: result.HttpStatus ?? 400);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPost("verify")]
    public async Task<ActionResult<VerifyOtpResponse>> Verify([FromBody] VerifyOtpGenericRequest request, CancellationToken ct)
    {
        var result = await _otpService.VerifyAsync(request, ct);
        if (!result.IsSuccess)
            return StatusCode(
                result.HttpStatus ?? 400,
                new VerifyOtpResponse { Success = false, Message = result.ErrorMessage ?? "" });

        return Ok(result.Value);
    }

    [HttpPost("resend")]
    public async Task<IActionResult> Resend([FromBody] ResendOtpGenericRequest request, CancellationToken ct)
    {
        var result = await _otpService.ResendAsync(request, ct);
        if (!result.IsSuccess)
            return Problem(
                title: result.ErrorCode ?? ErrorCodes.ValidationFailed,
                detail: result.ErrorMessage,
                statusCode: result.HttpStatus ?? 400);

        return Ok(result.Value);
    }
}
