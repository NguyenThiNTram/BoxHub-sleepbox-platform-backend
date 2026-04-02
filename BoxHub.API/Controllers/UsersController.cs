using BoxHub.Application.DTOs.Requests.Users;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/user")]
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
    [HttpPatch("me/profile")]
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

    [HttpPost("me/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> UploadAvatar(IFormFile avatar, CancellationToken ct)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        var result = await _userService.UploadAvatarAsync(userId.Value, avatar, ct);

        return Ok(result);
    }

    [HttpDelete("me")]
    public async Task<ActionResult> DeleteMe(CancellationToken ct)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        await _userService.SoftDeleteAccountAsync(userId.Value, ct);

        return NoContent();
    }

    [HttpPost("me/reactivate")]
    public async Task<ActionResult> ReactivateMe(CancellationToken ct)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized(ErrorFactory.Unauthorized(HttpContext));

        await _userService.ReactivateAccountAsync(userId.Value, ct);

        return Ok(new
        {
            message = "Account reactivated successfully"
        });
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
