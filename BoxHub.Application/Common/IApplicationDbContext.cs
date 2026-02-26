using BoxHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Application.Common;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

