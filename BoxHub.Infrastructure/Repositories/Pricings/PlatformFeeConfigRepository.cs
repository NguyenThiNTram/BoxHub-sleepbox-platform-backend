using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure.Repositories.Pricings
{
    public class PlatformFeeConfigRepository : IPlatformFeeConfigRepository
    {
        private readonly BoxHubDbContext _db;
        public PlatformFeeConfigRepository(BoxHubDbContext db)
        {
            _db = db;
        }

        public async Task<List<platform_fee_config>> GetAllAsync(CancellationToken ct)
        {
            return await _db.platform_fee_configs
                .AsNoTracking()
                .OrderByDescending(x => x.created_at)
                .ToListAsync(ct);
        }

        public async Task<platform_fee_config?> GetByIdAsync(Guid configId, CancellationToken ct)
        {
            return await _db.platform_fee_configs
                .FirstOrDefaultAsync(x => x.config_id == configId, ct);
        }

        public async Task AddAsync(platform_fee_config entity, CancellationToken ct)
        {
            await _db.platform_fee_configs.AddAsync(entity, ct);
        }

        public void Update(platform_fee_config entity)
        {
            _db.platform_fee_configs.Update(entity);
        }

        //Lấy config đang hiệu lực NOW
        public async Task<List<platform_fee_config>> GetCurrentConfigsAsync(DateTime now, CancellationToken ct)
        {
            var configs = await BaseQuery()
                .Where(x =>
                    x.is_active == true &&
                    x.effective_from <= now &&
                    (x.effective_to == null || x.effective_to >= now)
            ).ToListAsync(ct);

            return configs
                .GroupBy(x => x.fee_code)
                .Select(g =>
                    g.OrderByDescending(x => x.effective_from).First()
                ).ToList();
        }

        public async Task<List<platform_fee_config>> GetActiveConfigsAtAsync(DateTime atTime, Guid? hostId, CancellationToken ct)
        {
            var query = BaseQuery()
                .Where(x =>
                    x.is_active == true && x.effective_from <= atTime &&
                    (x.effective_to == null || x.effective_to >= atTime)
                );

            if (hostId.HasValue)
                query = query.Where(x => x.target_host_id == hostId.Value);
            else
                query = query.Where(x => x.target_host_id == null);

            return await query.ToListAsync(ct);
        }

        public async Task<platform_fee_config?> ResolveConfigAsync(FeeCode feeCode, Guid hostId, DateTime atTime, CancellationToken ct)
        {
            // 1. Host-specific
            var hostConfig = await BaseQuery()
                .Where(x =>
                    x.fee_code == feeCode &&
                    x.target_host_id == hostId &&
                    x.is_active == true &&
                    x.effective_from <= atTime &&
                    (x.effective_to == null || x.effective_to >= atTime)
                )
                .OrderByDescending(x => x.effective_from)
                .FirstOrDefaultAsync(ct);

            if (hostConfig != null)
                return hostConfig;

            // 2. Global fallback
            return await BaseQuery()
                .Where(x =>
                    x.fee_code == feeCode &&
                    x.target_host_id == null &&
                    x.is_active == true &&
                    x.effective_from <= atTime &&
                    (x.effective_to == null || x.effective_to >= atTime)
                )
                .OrderByDescending(x => x.effective_from)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<platform_fee_config>> GetHistoryByFeeCodeAsync(FeeCode feeCode, Guid? hostId, CancellationToken ct)
        {
            var query = BaseQuery()
                .Where(x => x.fee_code == feeCode);

            if (hostId.HasValue)
                query = query.Where(x => x.target_host_id == hostId.Value);
            else
                query = query.Where(x => x.target_host_id == null);

            return await query
                .OrderByDescending(x => x.effective_from)
                .ToListAsync(ct);
        }

        public async Task<bool> HasOverlapAsync(FeeCode feeCode, DateTime effectiveFrom, Guid? hostId, CancellationToken ct)
        {
            var query = BaseQuery()
                .Where(x =>
                    x.fee_code == feeCode &&
                    x.effective_from == effectiveFrom
                );

            if (hostId.HasValue)
                query = query.Where(x => x.target_host_id == hostId.Value);
            else
                query = query.Where(x => x.target_host_id == null);

            return await query.AnyAsync(ct);
        }

        public async Task<List<platform_fee_config>> ResolveAllConfigsAsync(Guid hostId, DateTime atTime, CancellationToken ct)
        {
            var configs = await BaseQuery()
                .Where(x =>
                    x.is_active == true &&
                    x.effective_from <= atTime &&
                    (x.effective_to == null || x.effective_to >= atTime)
                )
                .ToListAsync(ct);

            return configs
                .GroupBy(x => x.fee_code)
                .Select(g =>
                    g.Where(x => x.target_host_id == hostId)
                     .OrderByDescending(x => x.effective_from)
                     .FirstOrDefault()
                    ??
                    g.Where(x => x.target_host_id == null)
                     .OrderByDescending(x => x.effective_from)
                     .FirstOrDefault()
                )
                .Where(x => x != null)
                .ToList()!;
        }

        public async Task<bool> HasOverlapAdvancedAsync(FeeCode feeCode, DateTime effectiveFrom, DateTime? effectiveTo, Guid? hostId, CancellationToken ct)
        {
            var query = BaseQuery()
                .Where(x => x.fee_code == feeCode);

            if (hostId.HasValue)
                query = query.Where(x => x.target_host_id == hostId.Value);
            else
                query = query.Where(x => x.target_host_id == null);

            return await query.AnyAsync(x =>
                x.effective_from < (effectiveTo ?? DateTime.MaxValue) &&
                (x.effective_to ?? DateTime.MaxValue) > effectiveFrom
            , ct);
        }

        private IQueryable<platform_fee_config> BaseQuery()
        {
            return _db.platform_fee_configs.AsNoTracking();
        }

        
    }
}
