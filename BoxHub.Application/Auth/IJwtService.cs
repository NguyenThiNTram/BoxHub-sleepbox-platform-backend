using BoxHub.Application.DTOs.Responses;
using BoxHub.Domain.Entities;

namespace BoxHub.Application.Auth;

public interface IJwtService
{
    AuthResponse GenerateAccessToken(user user);
}