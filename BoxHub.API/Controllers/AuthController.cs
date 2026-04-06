using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("guest/register")]
    //[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterGuestRequest request,
        CancellationToken ct)
    {
        // Validate confirm password (presentation-level validation)
        if (request.Password != request.ConfirmPassword)
        {
            ModelState.AddModelError(
                nameof(request.ConfirmPassword),
                "Passwords do not match.");

            return ValidationProblem(ModelState);
        }

        var result = await _authService.RegisterGuestAsync(request, ct);

        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    //[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await _authService.AuthenticateAsync(
            request.Email,
            request.Password,
            ct);

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await _authService.LogoutAsync(ct);
        return Ok(new { success = true, message = "Đã đăng xuất." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await _authService.ForgotPasswordAsync(request, ct);
        return Ok(new { success = true, message = "Nếu email hợp lệ, một OTP đã được gửi đến bạn." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await _authService.ResetPasswordAsync(request, ct);
        return Ok(new { success = true, message = "Đổi mật khẩu thành công." });
    }
}