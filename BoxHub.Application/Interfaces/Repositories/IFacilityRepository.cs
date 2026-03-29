using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using BoxHub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories;

public interface IFacilityRepository
{
    Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(FacilitySearchRequest request, CancellationToken ct);

    /// <summary>Brand đang hoạt động của host gắn với user (Host đăng nhập).</summary>
    Task<Guid?> GetBrandIdForHostUserAsync(Guid userId, CancellationToken ct);

    Task AddFacilityAsync(facility entity, CancellationToken ct);

    Task AddFacilityDocumentsAsync(IReadOnlyList<facility_document> documents, CancellationToken ct);
}
