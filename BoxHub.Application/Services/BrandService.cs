using BoxHub.Application.DTOs.Requests.Brands;
using BoxHub.Application.DTOs.Responses.Brands;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Services;

public sealed class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICloudinaryService _cloudinary;

    public BrandService(IBrandRepository repo, IUnitOfWork uow, ICloudinaryService cloudinary)
    {
        _repo = repo;
        _uow = uow;
        _cloudinary = cloudinary;
    }

    public async Task<Result<BrandResponse>> UpdateBrandAsync(
        Guid hostUserId,
        BrandRequest request,
        CancellationToken ct)
    {
        var hasAvatarUpload = request.BrandAvatar is { Length: > 0 };
        if (request.BrandName is null && !hasAvatarUpload)
        {
            return Result<BrandResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Cần gửi ít nhất tên thương hiệu hoặc ảnh logo để cập nhật.",
                400);
        }

        var hostId = await _repo.GetHostIdByUserIdAsync(hostUserId, ct);
        if (hostId is null)
        {
            return Result<BrandResponse>.Failure(
                ErrorCodes.Forbidden,
                "Tài khoản không có hồ sơ Host.",
                403);
        }

        var entity = await _repo.GetByHostIdAsync(hostId.Value, track: true, ct);
        if (entity is null)
        {
            return Result<BrandResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Không tìm thấy thương hiệu của Host.",
                404);
        }

        if (request.BrandName is not null)
        {
            var name = request.BrandName.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<BrandResponse>.Failure(
                    ErrorCodes.ValidationFailed,
                    "Tên thương hiệu không được để trống.",
                    400);
            }

            var norm = name.ToUpperInvariant();
            var currentNorm = entity.brand_name.ToUpperInvariant();
            if (!string.Equals(norm, currentNorm, StringComparison.Ordinal))
            {
                var taken = await _repo.CheckExistsBrandName(entity.brand_id, norm, ct);
                if (taken)
                {
                    return Result<BrandResponse>.Failure(
                        ErrorCodes.ValidationFailed,
                        "Tên thương hiệu đã được sử dụng.",
                        409);
                }
            }

            entity.brand_name = name;
        }

        if (hasAvatarUpload)
        {
            var url = await _cloudinary.UploadImageAsync(request.BrandAvatar!);
            if (!string.IsNullOrWhiteSpace(url))
                entity.brand_avatar = url;
        }

        entity.updated_at = DateTime.UtcNow;

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<BrandResponse>.Success(Map(entity));
    }

    private static BrandResponse Map(brand b) => new()
    {
        BrandId = b.brand_id,
        HostId = b.host_id,
        BrandName = b.brand_name,
        BrandAvatar = b.brand_avatar,
        Status = b.status
    };
}
