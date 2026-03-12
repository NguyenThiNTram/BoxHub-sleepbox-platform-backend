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
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.user_id == id, ct);
    }
}
