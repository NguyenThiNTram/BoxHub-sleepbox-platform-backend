using BoxHub.Application.DTOs.Requests.Admins;
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
    }
}
