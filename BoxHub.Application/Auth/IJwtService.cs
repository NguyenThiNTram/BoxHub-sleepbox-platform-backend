using BoxHub.Domain.Entities;

namespace BoxHub.Application.Auth;

public interface IJwtService
{
    AuthResponse GenerateAccessToken(User user);
}

