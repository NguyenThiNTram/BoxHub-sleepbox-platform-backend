using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/host/facilities")]
[Authorize(Roles = "HOST")]
public sealed class HostFacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public HostFacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    /// <summary>Tạo cơ sở mới kèm upload 2 giấy tờ: BUSINESS_LICENSE và PCCC (mỗi loại một bản / cơ sở).</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateFacilityRequest request, CancellationToken ct)
    {
        var userId = GetHostUserId();
        if (userId is null)
            return ProblemResult(ErrorCodes.Unauthorized, "Token không hợp lệ hoặc thiếu thông tin người dùng.", 401);

        var result = await _facilityService.CreateForHostAsync(userId.Value, request, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    private Guid? GetHostUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(sub, out var id) ? id : null;
    }

    private static ObjectResult ProblemResult(string? code, string? message, int status) =>
        new(new ProblemDetails
        {
            Title = code ?? ErrorCodes.ValidationFailed,
            Detail = message,
            Status = status
        })
        {
            StatusCode = status
        };
}
