using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Infrastructure.Repositories;

public sealed class AmenityRepository : IAmenityRepository
{
    private readonly BoxHubDbContext _db;

    public AmenityRepository(BoxHubDbContext db)
    {
        _db = db;
    }

    public async Task<(List<amenity> Items, int TotalCount)> ListAsync(
        string? search,
        string? type,
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var query = _db.amenities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            var t = type.Trim();
            query = query.Where(a => a.amenity_type != null && a.amenity_type == t);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            query = query.Where(a =>
                EF.Functions.ILike(a.amenity_name, $"%{q}%") ||
                EF.Functions.ILike(a.description, $"%{q}%"));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(a => a.amenity_name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<amenity?> GetByIdAsync(int id, bool track, CancellationToken ct)
    {
        var query = track ? _db.amenities.AsQueryable() : _db.amenities.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.amenity_id == id, ct);
    }

    public async Task<amenity?> GetByNameAsync(string nameNorm, bool track, CancellationToken ct)
    {
        var query = track ? _db.amenities.AsQueryable() : _db.amenities.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.amenity_name.ToUpper() == nameNorm, ct);
    }

    public async Task AddAsync(amenity entity, CancellationToken ct)
    {
        await _db.amenities.AddAsync(entity, ct);
    }

    public Task UpdateAsync(amenity entity, CancellationToken ct)
    {
        _ = ct;
        _db.amenities.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(amenity entity, CancellationToken ct)
    {
        _ = ct;
        _db.amenities.Remove(entity);
        return Task.CompletedTask;
    }
}

