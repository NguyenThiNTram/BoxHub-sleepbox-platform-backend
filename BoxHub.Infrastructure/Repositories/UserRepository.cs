using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Requests.Moderators;
using BoxHub.Application.DTOs.Responses.Moderators;
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
                //.AsNoTracking()
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
                .Include(u => u.user_profile)
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

        public async Task<(IEnumerable<AccountListItem>, int)>GetUsersForModeratorAsync(AccountFilter filter, CancellationToken ct)
        {
            var query = _db.users
                .AsNoTracking()
                .AsQueryable();

            // role filter
            if (filter.Role.HasValue)
            {
                query = query.Where(x => x.role == filter.Role.Value);
            }

            // status filter
            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.user_status == filter.Status.Value);
            }

            // search
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();

                query = query.Where(x =>
                    EF.Functions.ILike(x.username, $"%{keyword}%") ||
                    EF.Functions.ILike(x.email, $"%{keyword}%"));
            }

            // total count before paging
            var totalCount = await query.CountAsync(ct);

            // ===== PAGING + PROJECTION =====
            var items = await query
                .OrderByDescending(x => x.created_at)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new AccountListItem
                {
                    UserId = x.user_id,
                    Username = x.username,
                    Email = x.email,
                    Phone = x.phone,
                    Role = x.role,
                    Status = x.user_status,
                    IsEmailVerified = x.is_email_verified,
                    LastLoginAt = x.last_login_at,
                    CreatedAt = x.created_at
                })
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<AccountDetail?>GetUserDetailForModeratorAsync(Guid userId, CancellationToken ct)
        {
            var result = await _db.users
                .AsNoTracking()
                .Where(x => x.user_id == userId)
                .Select(x => new AccountDetail
                {
                    UserId = x.user_id,
                    Username = x.username,
                    Email = x.email,
                    Phone = x.phone,
                    Role = x.role,
                    Status = x.user_status,
                    IsEmailVerified = x.is_email_verified,
                    EmailVerifiedAt = x.email_verified_at,
                    LastLoginAt = x.last_login_at,
                    CreatedAt = x.created_at,
                    DeletedAt = x.deleted_at,

                    // ===== GUEST PROFILE =====
                    GuestProfile = _db.user_profiles
                        .Where(p => p.user_id == x.user_id)
                        .Select(p => new GuestProfile
                        {
                            FirstName = p.first_name,
                            LastName = p.last_name,
                            Gender = p.gender,
                            DateOfBirth = p.date_of_birth,
                            AvatarUrl = p.avatar_url
                        })
                        .FirstOrDefault(),

                    // ===== HOST PROFILE =====
                    HostProfile = _db.host_profiles
                        .Where(h => h.user_id == x.user_id)
                        .Select(h => new HostInfo
                        {
                            HostId = h.host_id,
                            RepresentativeName = h.representative_id_name,
                            RepresentativeIdNumber = h.representative_id_number,
                            TaxCode = h.tax_code,

                            BusinessName = h.business_name,
                            AddressDistrict = h.address_district,
                            AddressWard = h.address_ward,
                            AddressDetail = h.address_detail,

                            RepresentativeFrontUrl = h.representative_front_url,
                            RepresentativeBackUrl = h.representative_back_url,

                            SubmittedAt = h.submitted_at,
                            VerifiedAt = h.verified_at,
                            RejectReason = h.reject_reason,

                            VerifiedStatus =
                                h.verified_at != null
                                    ? "VERIFIED"
                                    : h.reject_reason != null
                                        ? "REJECTED"
                                        : "PENDING"
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);

            return result;
        }
    }
}