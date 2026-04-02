using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Infrastructure.Repositories;

public sealed class HostPayoutRepository : IHostPayoutRepository
{
    private readonly BoxHubDbContext _db;

    public HostPayoutRepository(BoxHubDbContext db)
    {
        _db = db;
    }

    public async Task<host_profile?> GetHostProfileWithPayoutByUserIdAsync(Guid userId, bool track, CancellationToken ct)
    {
        var q = _db.host_profiles
            .Include(h => h.host_payout_account)
            .AsQueryable();

        if (!track)
            q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(h => h.user_id == userId, ct);
    }

    public Task AddPayoutAsync(host_payout_account account, CancellationToken ct) =>
        _db.host_payout_accounts.AddAsync(account, ct).AsTask();
}
