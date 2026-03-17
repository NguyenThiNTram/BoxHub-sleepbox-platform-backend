using BoxHub.Application.DTOs.Responses;
using BoxHub.Domain.Entities;

namespace BoxHub.Application.Interfaces;

public interface IJwtService
{
    AuthResponse GenerateAccessToken(user user);
}