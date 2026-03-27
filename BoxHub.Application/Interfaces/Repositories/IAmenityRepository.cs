using BoxHub.Domain.Entities;

namespace BoxHub.Application.Interfaces.Repositories;

public interface IAmenityRepository
{
    Task<(List<amenity> Items, int TotalCount)> ListAsync(
        string? search,
        string? type,
        int pageNumber,
        int pageSize,
        CancellationToken ct);

    Task<amenity?> GetByIdAsync(int id, bool track, CancellationToken ct);

    Task<amenity?> GetByNameAsync(string nameNorm, bool track, CancellationToken ct);

    Task AddAsync(amenity entity, CancellationToken ct);

    Task UpdateAsync(amenity entity, CancellationToken ct);

    Task DeleteAsync(amenity entity, CancellationToken ct);
}

