using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using BoxHub.Shared.Results;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services;

public interface IFacilityService
{
    Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(FacilitySearchRequest request, CancellationToken ct);

    /// <summary>Tạo cơ sở cho brand của Host; bắt buộc 2 file giấy tờ (BUSINESS_LICENSE, PCCC).</summary>
    Task<Result<CreateFacilityResponse>> CreateForHostAsync(Guid hostUserId, CreateFacilityRequest request, CancellationToken ct);
}
