using BoxHub.Domain.Entities;

namespace BoxHub.Application.Interfaces.Repositories;

public interface IHostPayoutRepository
{
    Task<host_profile?> GetHostProfileWithPayoutByUserIdAsync(Guid userId, bool track, CancellationToken ct);

    Task AddPayoutAsync(host_payout_account account, CancellationToken ct);
}
