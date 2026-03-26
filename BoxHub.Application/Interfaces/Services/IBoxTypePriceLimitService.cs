using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IBoxTypePriceLimitService
    {
        Task<ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>> GetAllAsync();

        Task<ApiResponse<BoxTypePriceLimitResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>> GetByCapacityAndClassAsync(
        CapacityType capacityType,
        BoxClass? boxClass);

        Task<ApiResponse<BoxTypePriceLimitResponse>> CreateAsync(CreateBoxTypePriceLimitRequest req, CancellationToken ct);

        Task<ApiResponse<BoxTypePriceLimitResponse>> UpdateAsync(Guid id, UpdateBoxTypePriceLimitRequest req, CancellationToken ct);

        Task<ApiResponse<bool>> ToggleActiveAsync(Guid id, CancellationToken ct);

        Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct);
    }
}
