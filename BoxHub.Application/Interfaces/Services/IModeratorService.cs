using BoxHub.Application.DTOs.Requests.Moderators;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Moderators;
using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IModeratorService
    {
        Task<PagedResponse<AccountListItem>>GetUsersAsync(AccountFilter filter, CancellationToken ct);

        Task<AccountDetail?>GetUserDetailAsync(Guid userId, CancellationToken ct);

        Task<SuspendAccountResult>SuspendUserAsync(Guid userId, SuspendAccountRequest request, Guid actorId, UserRole actorRole, CancellationToken ct);
    }
}
