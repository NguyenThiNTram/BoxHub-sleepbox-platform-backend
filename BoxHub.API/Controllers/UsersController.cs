using BoxHub.Application.DTOs.Requests.Users;
using BoxHub.Application.Interfaces;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/auth/guest")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetMe(CancellationToken ct)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        var result = await _userService.GetCurrentUserAsync(userId.Value, ct);

        return Ok(result);
    }

    [HttpPut("me/profile")]
    public async Task<ActionResult> UpdateMeProfile(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        var result = await _userService.UpdateUserProfileAsync(userId.Value, request, ct);

        return Ok(result);
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}