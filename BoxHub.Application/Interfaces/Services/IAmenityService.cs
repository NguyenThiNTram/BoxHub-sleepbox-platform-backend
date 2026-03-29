using BoxHub.Application.DTOs.Requests.Amenities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Amenities;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Interfaces.Services;

public interface IAmenityService
{
    Task<Result<PagedResponse<AmenityResponse>>> ListAsync(AmenityListRequest request, CancellationToken ct);

    Task<Result<AmenityResponse>> GetByIdAsync(int id, CancellationToken ct);

    Task<Result<AmenityResponse>> CreateAsync(AmenityRequest request, CancellationToken ct);

    Task<Result<AmenityResponse>> UpdateAsync(int id, AmenityRequest request, CancellationToken ct);

    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct);
}

