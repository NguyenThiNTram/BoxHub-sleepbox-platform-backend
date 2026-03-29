using BoxHub.Domain.Entities;

namespace BoxHub.Application.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<Guid?> GetHostIdByUserIdAsync(Guid userId, CancellationToken ct);

    Task<brand?> GetByHostIdAsync(Guid hostId, bool track, CancellationToken ct);

    Task<bool> CheckExistsBrandName(Guid excludeBrandId, string normalizedName, CancellationToken ct);

    Task UpdateAsync(brand entity, CancellationToken ct);
}
