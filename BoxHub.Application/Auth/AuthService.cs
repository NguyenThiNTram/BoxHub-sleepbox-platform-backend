using BoxHub.Application.Common;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Application.Auth;

public sealed class AuthService : IAuthService
{
    private const string GuestRole = "GUEST";
    private const string ActiveStatus = "ACTIVE";

    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IApplicationDbContext dbContext,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> RegisterGuestAsync(RegisterGuestRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (existingUser != null)
        {
            return null;
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = normalizedEmail,
            Username = BuildUsernameFromEmail(normalizedEmail),
            Role = UserRole.GUEST,
            UserStatus = ActiveStatus,
            CreatedAt = DateTime.UtcNow,
            IsEmailVerified = false
        };

        user.PasswordHash = _passwordService.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _jwtService.GenerateAccessToken(user);
    }

    public async Task<AuthResponse?> LoginGuestAsync(LoginGuestRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return null;
        }

        if (user.Role != UserRole.GUEST)
        {
            return null;
        }

        if (!string.Equals(user.UserStatus, ActiveStatus, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !_passwordService.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _jwtService.GenerateAccessToken(user);
    }

    private static string BuildUsernameFromEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex > 0 ? email[..atIndex] : email;
    }

}

