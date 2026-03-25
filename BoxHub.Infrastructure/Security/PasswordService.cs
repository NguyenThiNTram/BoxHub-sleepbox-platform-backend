using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BoxHub.Infrastructure.Auth;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
        => _hasher.HashPassword(null!, password);

    public bool VerifyPassword(string hashedPassword, string inputPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashedPassword, inputPassword);
        return result != PasswordVerificationResult.Failed;
    }
}