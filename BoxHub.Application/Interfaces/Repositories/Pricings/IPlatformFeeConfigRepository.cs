using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories.Pricings
{
    public interface IPlatformFeeConfigRepository
    {
        Task<List<platform_fee_config>> GetAllAsync(CancellationToken ct);

        Task<platform_fee_config?> GetByIdAsync(Guid configId, CancellationToken ct);

        Task AddAsync(platform_fee_config entity, CancellationToken ct);

        void Update(platform_fee_config entity);

        Task<List<platform_fee_config>> GetCurrentConfigsAsync(DateTime now, CancellationToken ct);

        Task<List<platform_fee_config>> GetActiveConfigsAtAsync(
            DateTime atTime,
            Guid? hostId,
            CancellationToken ct
        );

        Task<platform_fee_config?> ResolveConfigAsync(
            FeeCode feeCode,
            Guid hostId,
            DateTime atTime,
            CancellationToken ct
        );

        Task<List<platform_fee_config>> GetHistoryByFeeCodeAsync(
            FeeCode feeCode,
            Guid? hostId,
            CancellationToken ct
        );

        Task<bool> HasOverlapAsync(
            FeeCode feeCode,
            DateTime effectiveFrom,
            Guid? hostId,
            CancellationToken ct
        );

        Task<List<platform_fee_config>> ResolveAllConfigsAsync(
            Guid hostId,
            DateTime atTime,
            CancellationToken ct
        );
    }
}
