using BoxHub.Application.DTOs.Requests.Brands;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/host/brand")]
[Authorize(Roles = "HOST")]
public sealed class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>Cập nhật brand của Host đang đăng nhập (mỗi Host một brand). Gửi multipart/form-data: BrandName (optional), BrandAvatar (file ảnh, optional).</summary>
    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateBrand([FromForm] BrandRequest request, CancellationToken ct)
    {
        var userId = GetHostUserId();
        if (userId is null)
            return ProblemResult(ErrorCodes.Unauthorized, "Token không hợp lệ hoặc thiếu thông tin người dùng.", 401);

        var result = await _brandService.UpdateBrandAsync(userId.Value, request, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
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
