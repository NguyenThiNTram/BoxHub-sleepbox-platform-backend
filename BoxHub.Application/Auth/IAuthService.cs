using BoxHub.Application.Auth;

namespace BoxHub.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse?> RegisterGuestAsync(RegisterGuestRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> LoginGuestAsync(LoginGuestRequest request, CancellationToken cancellationToken = default);
}

