using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services.Pricings
{
    public interface IAddonServiceService
    {
        Task<ApiResponse<IEnumerable<AddonService>>> GetAllAsync(bool? isActive, CancellationToken ct);

        Task<ApiResponse<AddonService>> CreateAsync(CreateAddonServiceRequest req, CancellationToken ct);

        Task<ApiResponse<AddonService>> UpdateAsync(Guid serviceId, UpdateAddonServiceRequest req, CancellationToken ct);

        Task<ApiResponse<bool>> ToggleActiveAsync(Guid serviceId, CancellationToken ct);

        Task<ApiResponse<bool>> DeleteAsync(Guid serviceId, CancellationToken ct);
    }
}
