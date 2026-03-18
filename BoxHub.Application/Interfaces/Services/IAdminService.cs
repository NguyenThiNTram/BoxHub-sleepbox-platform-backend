using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Admins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<CreateAdminResponse> CreateAdminAsync(Guid currentAdminId, CreateAdminRequest request, CancellationToken ct);
        Task<PagedResponse<UserItemResponse>> GetUsersAsync(Guid currentAdminId, GetUsersRequest request, CancellationToken ct);
        Task SuspendUserAsync(Guid currentAdminId, Guid targetUserId, CancellationToken ct);
        Task<UserItemResponse> CreateModeratorAsync(Guid currentAdminId, CreateModeratorRequest request, CancellationToken ct);
        Task<UserItemResponse> UpdateModeratorAsync(Guid currentAdminId, Guid moderatorId, UpdateModeratorRequest request, CancellationToken ct);
    }
}
