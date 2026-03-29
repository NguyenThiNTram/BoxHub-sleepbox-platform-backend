using BoxHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories.Pricings
{
    public interface IAddonServiceRepository
    {
            // basic
            Task<addon_service?> GetByIdAsync(Guid id, CancellationToken ct);

            Task<IEnumerable<addon_service>> GetAllAsync(CancellationToken ct);

            Task AddAsync(addon_service entity, CancellationToken ct);

            void Update(addon_service entity);

            void Delete(addon_service entity);

            // custom
            Task<addon_service?> GetByNameAsync(string serviceName, CancellationToken ct);

            Task<IEnumerable<addon_service>> GetActiveAsync(CancellationToken ct);

            Task<bool> ExistsAsync(Expression<Func<addon_service, bool>> predicate, CancellationToken ct);

            Task<bool> IsUsedByHostAsync(Guid serviceId, CancellationToken ct);
        }
    }
