using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BoxHubDbContext _db;

        public UserRepository(BoxHubDbContext db)
        {
            _db = db;
        }

        public async Task<user?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _db.users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == email, ct);
        }

        public async Task<user?> GetByUsernameAsync(string username, CancellationToken ct)
        {
            return await _db.users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.username == username, ct);
        }

        public async Task<user?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.users
                .FirstOrDefaultAsync(u => u.user_id == id, ct);
        }

        public async Task<user_profile?> GetProfileAsync(Guid userId, CancellationToken ct)
        {
            return await _db.user_profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.user_id == userId, ct);
        }

        public async Task<(IEnumerable<user> Items, int TotalCount)> GetUsersAsync(
            GetUsersRequest request,
            CancellationToken ct)
        {
            var query = _db.users
                .AsNoTracking()
                .Include(u => u.user_profile)
                .AsQueryable();

            if (request.Role.HasValue)
            {
                query = query.Where(x => x.role == request.Role.Value);
            }

            if (request.UserStatus.HasValue)
            {
                query = query.Where(x => x.user_status == request.UserStatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var keyword = request.SearchTerm.Trim();

                query = query.Where(x =>
                    EF.Functions.ILike(x.email, $"%{keyword}%") ||
                    EF.Functions.ILike(x.username, $"%{keyword}%"));
            }

            if (request.CreatedFrom.HasValue)
            {
                query = query.Where(x => x.created_at >= request.CreatedFrom.Value);
            }

            if (request.CreatedTo.HasValue)
            {
                query = query.Where(x => x.created_at <= request.CreatedTo.Value);
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.created_at)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task AddAsync(user user, CancellationToken ct)
        {
            return _db.users.AddAsync(user, ct).AsTask();
        }

        public Task CreateProfileAsync(user_profile profile, CancellationToken ct)
        {
            return _db.user_profiles.AddAsync(profile, ct).AsTask();
        }

        public Task AddAuditLogAsync(audit_log auditLog, CancellationToken ct)
        {
            return _db.audit_logs.AddAsync(auditLog, ct).AsTask();
        }

        public Task UpdateAsync(user user, CancellationToken ct)
        {
            _db.users.Update(user);
            return Task.CompletedTask;
        }

        public Task UpdateProfileAsync(user_profile profile, CancellationToken ct)
        {
            _db.user_profiles.Update(profile);
            return Task.CompletedTask;
        }

        //public Task SaveChangesAsync(CancellationToken ct)
        //{
        //    return _db.SaveChangesAsync(ct);
        //}
    }
}