using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Infrastructure.Repositories;

public sealed class BrandRepository : IBrandRepository
{
    private readonly BoxHubDbContext _db;

    public BrandRepository(BoxHubDbContext db)
    {
        _db = db;
    }

    public async Task<Guid?> GetHostIdByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.host_profiles
            .AsNoTracking()
            .Where(h => h.user_id == userId)
            .Select(h => (Guid?)h.host_id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<brand?> GetByHostIdAsync(Guid hostId, bool track, CancellationToken ct)
    {
        var q = track ? _db.brands.AsQueryable() : _db.brands.AsNoTracking();
        return await q.FirstOrDefaultAsync(b => b.host_id == hostId && !b.is_deleted, ct);
    }

    public Task<bool> CheckExistsBrandName(
        Guid excludeBrandId,
        string normalizedName,
        CancellationToken ct)
    {
        return _db.brands.AnyAsync(
            b => !b.is_deleted
                 && b.brand_id != excludeBrandId
                 && b.brand_name.ToUpper() == normalizedName,
            ct);
    }

    public Task UpdateAsync(brand entity, CancellationToken ct)
    {
        _ = ct;
        _db.brands.Update(entity);
        return Task.CompletedTask;
    }
}
