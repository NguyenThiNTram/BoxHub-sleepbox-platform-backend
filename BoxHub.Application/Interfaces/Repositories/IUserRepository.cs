using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<user?> GetByEmailAsync(string email, CancellationToken ct);
        Task<user?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(user user, CancellationToken ct);
        //Task SaveChangesAsync(CancellationToken ct);
        Task<(IEnumerable<user> Items, int TotalCount)> GetUsersAsync( GetUsersRequest request, CancellationToken ct);
        Task AddAuditLogAsync(audit_log auditLog, CancellationToken ct);
        Task UpdateAsync(user user, CancellationToken ct);

        //--- user profile related methods
        Task<user_profile?> GetProfileAsync(Guid userId, CancellationToken ct);
        Task CreateProfileAsync(user_profile profile, CancellationToken ct);
        Task UpdateProfileAsync(user_profile profile, CancellationToken ct);
        
    }
}
