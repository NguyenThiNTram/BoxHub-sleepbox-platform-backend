using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterGuestAsync(RegisterGuestRequest request, CancellationToken ct);
        Task<AuthResponse?> LoginGuestAsync(LoginGuestRequest request, CancellationToken ct);
    }
}
