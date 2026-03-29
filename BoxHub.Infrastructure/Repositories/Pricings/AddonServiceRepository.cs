using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure.Repositories.Pricings
{
    public class AddonServiceRepository : IAddonServiceRepository
    {
        private readonly BoxHubDbContext _db;

        public AddonServiceRepository(BoxHubDbContext db)
        {
            _db = db;
        }
        public async Task<addon_service?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.addon_services
                .FirstOrDefaultAsync(x => x.service_id == id, ct);
        }

        public async Task<IEnumerable<addon_service>> GetAllAsync(CancellationToken ct)
        {
            return await _db.addon_services
                .AsNoTracking()
                .OrderBy(x => x.service_name)
                .ToListAsync(ct);
        }

        public async Task AddAsync(addon_service entity, CancellationToken ct)
        {
            await _db.addon_services.AddAsync(entity, ct);
        }

        public void Update(addon_service entity)
        {
            _db.addon_services.Update(entity);
        }

        public void Delete(addon_service entity)
        {
            _db.addon_services.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<addon_service, bool>> predicate, CancellationToken ct)
        {
            return await _db.addon_services.AnyAsync(predicate, ct);
        }

        public async Task<addon_service?> GetByNameAsync(string serviceName, CancellationToken ct)
        {
            var normalized = serviceName.Trim().ToLower();

            return await _db.addon_services
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.service_name.ToLower() == normalized, ct);
        }

        public async Task<IEnumerable<addon_service>> GetActiveAsync(CancellationToken ct)
        {
            return await _db.addon_services
                .AsNoTracking()
                .Where(x => x.is_active == true)
                .OrderBy(x => x.service_name)
                .ToListAsync(ct);
        }

        public async Task<bool> IsUsedByHostAsync(Guid serviceId, CancellationToken ct)
        {
            return await _db.host_addon_prices
                .AsNoTracking()
                .AnyAsync(x => x.service_id == serviceId, ct);
        }

        public async Task<int> CountFacilitiesUsingAsync(Guid serviceId, CancellationToken ct = default)
        {
            return await _db.host_addon_prices
                .Where(h => h.service_id == serviceId)
                .Select(h => h.facility_id)
                .Distinct()
                .CountAsync(ct);
        }

    }
}
