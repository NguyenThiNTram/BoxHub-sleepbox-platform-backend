using BoxHub.Application.DTOs.Requests.Amenities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Amenities;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Services;

public sealed class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _repo;
    private readonly IUnitOfWork _uow;

    public AmenityService(IAmenityRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<PagedResponse<AmenityResponse>>> ListAsync(AmenityListRequest request, CancellationToken ct)
    {
        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var size = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;
        var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();
        var type = string.IsNullOrWhiteSpace(request.Type) ? null : request.Type.Trim();

        var (items, total) = await _repo.ListAsync(search, type, page, size, ct);
        var mapped = items.Select(Map).ToList();

        return Result<PagedResponse<AmenityResponse>>.Success(new PagedResponse<AmenityResponse>(mapped, total, page, size));
    }

    public async Task<Result<AmenityResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        if (id <= 0)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "amenityId không hợp lệ.", 400);

        var a = await _repo.GetByIdAsync(id, track: false, ct);
        if (a == null)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Không tìm thấy tiện ích.", 404);

        return Result<AmenityResponse>.Success(Map(a));
    }

    public async Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request, CancellationToken ct)
    {
        var name = (request.Name ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Tên tiện ích không được để trống.", 400);

        var norm = name.ToUpperInvariant();
        var existing = await _repo.GetByNameAsync(norm, track: false, ct);
        if (existing != null)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Tên tiện ích đã tồn tại.", 409);

        var entity = new amenity
        {
            amenity_name = name,
            amenity_type = string.IsNullOrWhiteSpace(request.Type) ? null : request.Type.Trim(),
            description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<AmenityResponse>.Success(Map(entity));
    }

    public async Task<Result<AmenityResponse>> UpdateAsync(int id, UpdateAmenityRequest request, CancellationToken ct)
    {
        if (id <= 0)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "amenityId không hợp lệ.", 400);

        var entity = await _repo.GetByIdAsync(id, track: true, ct);
        if (entity == null)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Không tìm thấy tiện ích.", 404);

        var name = (request.Name ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Tên tiện ích không được để trống.", 400);

        var norm = name.ToUpperInvariant();
        var byName = await _repo.GetByNameAsync(norm, track: false, ct);
        if (byName != null && byName.amenity_id != id)
            return Result<AmenityResponse>.Failure(ErrorCodes.ValidationFailed, "Tên tiện ích đã tồn tại.", 409);

        entity.amenity_name = name;
        entity.amenity_type = string.IsNullOrWhiteSpace(request.Type) ? null : request.Type.Trim();
        entity.description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<AmenityResponse>.Success(Map(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct)
    {
        if (id <= 0)
            return Result<bool>.Failure(ErrorCodes.ValidationFailed, "amenityId không hợp lệ.", 400);

        var entity = await _repo.GetByIdAsync(id, track: true, ct);
        if (entity == null)
            return Result<bool>.Failure(ErrorCodes.ValidationFailed, "Không tìm thấy tiện ích.", 404);

        await _repo.DeleteAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    private static AmenityResponse Map(amenity a) => new()
    {
        AmenityId = a.amenity_id,
        Name = a.amenity_name,
        Type = a.amenity_type,
        Description = a.description
    };
}

