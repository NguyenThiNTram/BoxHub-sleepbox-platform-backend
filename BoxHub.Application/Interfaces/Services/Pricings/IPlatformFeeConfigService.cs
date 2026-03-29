using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services.Pricings
{
    public interface IPlatformFeeConfigService
    {

        Task<ApiResponse<List<PlatformFeeConfig>>>GetAllActiveAsync(CancellationToken ct);

        //full history
        Task<ApiResponse<FeeTimeline>>GetTimelineAsync(FeeCode feeCode, Guid? hostId, CancellationToken ct);

        // Validate: EffectiveFrom >= Today
        // Validate: PercentageValue XOR FixedAmount (không được cả 2 cùng có)
        // Guard: unique (feeCode, effectiveFrom, targetHostId) → 409
        // Guard: Nếu TargetHostId có → verify host tồn tại và verified
        // NOTE: KHÔNG sửa config cũ — luôn tạo record mới (append-only)
        //       Config cũ vẫn giữ nguyên → pricing_snapshot bất biến
        Task<ApiResponse<PlatformFeeConfig>>CreateAsync(CreatePlatformFeeConfigRequest request, CancellationToken ct);

        // Chỉ tắt config tương lai (EffectiveFrom > NOW)
        // Config đã hiệu lực (EffectiveFrom <= NOW) → không cho tắt
        // Bảo vệ tính nhất quán snapshot
        Task<ApiResponse<bool>>DeactivateAsync(Guid configId, CancellationToken ct);

        // price calculation
        Task<ResolvedFees>ResolveFeesForBookingAsync(Guid hostId, DateTime bookingTime, CancellationToken ct);
    }
}
