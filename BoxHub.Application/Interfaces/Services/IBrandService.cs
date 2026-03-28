using BoxHub.Application.DTOs.Requests.Brands;
using BoxHub.Application.DTOs.Responses.Brands;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Interfaces.Services;

public interface IBrandService
{
    /// <summary>
    /// Host đăng nhập cập nhật đúng brand gắn với host_profile của user đó.
    /// </summary>
    Task<Result<BrandResponse>> UpdateBrandAsync(Guid hostUserId, BrandRequest request, CancellationToken ct);
}
