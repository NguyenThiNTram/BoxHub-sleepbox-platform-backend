using BoxHub.Domain.Entities;
using BoxHub.Infrastructure.Data;
using BoxHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .FirstOrDefaultAsync(u => u.email == email, ct);
        }

        public async Task AddAsync(user user, CancellationToken ct)
        {
            await _db.users.AddAsync(user, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            return _db.SaveChangesAsync(ct);
        }
        public async Task<user?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.users
            .FirstOrDefaultAsync(u => u.user_id == id, ct);

        public async Task<user_profile?> GetProfileAsync(Guid userId, CancellationToken ct)
        {
            return await _db.user_profiles
                .FirstOrDefaultAsync(p => p.user_id == userId, ct);
        }

        public async Task CreateProfileAsync(user_profile profile, CancellationToken ct)
        {
            await _db.user_profiles.AddAsync(profile, ct);
        }

        public Task UpdateProfileAsync(user_profile profile, CancellationToken ct)
        {
            _db.user_profiles.Update(profile);
            return Task.CompletedTask;
        }

        public Task UpdateUserAsync(user user, CancellationToken ct)
        {
            _db.users.Update(user);
            return Task.CompletedTask;
        }
    }
}
