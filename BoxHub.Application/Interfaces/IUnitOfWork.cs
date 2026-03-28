using BoxHub.Application.Interfaces.Repositories.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(CancellationToken ct);
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
        //IBoxTypePriceLimitRepository BoxTypePriceLimits { get; }
        Task SaveChangesAsync(CancellationToken ct);
    }
}
