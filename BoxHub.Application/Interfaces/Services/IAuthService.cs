using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(string email, string password, CancellationToken ct);
        Task<AuthResponse?> RegisterGuestAsync(RegisterGuestRequest request, CancellationToken ct);
        //Task<AuthResponse?> LoginGuestAsync(LoginRequest request, CancellationToken ct);
    }
}
