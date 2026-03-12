using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/auth/guest")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    //[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status409Conflict)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterGuestRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
        {
            ModelState.AddModelError(
                nameof(request.ConfirmPassword),
                "Passwords do not match.");

            return ValidationProblem(ModelState);
        }

        var result = await _authService
            .RegisterGuestAsync(request, cancellationToken);

        if (result is null)
        {
            return Conflict(new
            {
                error = "Email already exists."
            });
        }

        return Created("", result);
    }

    [HttpPost("login")]
    //[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginGuestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService
            .LoginGuestAsync(request, cancellationToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                error = "Invalid credentials or inactive account."
            });
        }

        return Ok(result);
    }
}