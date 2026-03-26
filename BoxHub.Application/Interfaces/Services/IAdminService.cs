using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Admins;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<Result<CreateAdminResponse>> CreateAdminAsync(Guid currentAdminId, CreateAdminRequest request, CancellationToken ct);

        Task<Result<UserItemResponse>> CreateModeratorAsync(Guid currentAdminId, CreateModeratorRequest request, CancellationToken ct);

        Task<Result<UserItemResponse>> UpdateModeratorAsync(Guid currentAdminId, Guid moderatorId, UpdateModeratorRequest request, CancellationToken ct);

        Task<Result<PagedResponse<UserItemResponse>>> GetUsersAsync(Guid currentAdminId, GetUsersRequest request, CancellationToken ct);

        Task<Result<SimpleMessageResponse>> SuspendUserAsync(Guid currentAdminId, Guid targetUserId, CancellationToken ct);
    }
}
