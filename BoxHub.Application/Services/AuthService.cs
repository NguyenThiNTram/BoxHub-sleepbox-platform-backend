using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private const UserRole GuestRole = UserRole.Guest;
        private const UserStatus ActiveStatus = UserStatus.Active;

        private readonly IUserRepository _users;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository users,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _users = users;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse?> RegisterGuestAsync(
            RegisterGuestRequest request,
            CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existing = await _users.GetByEmailAsync(email, ct);

            if (existing != null)
                return null;

            var newUser = new user
            {
                user_id = Guid.NewGuid(),
                email = email,
                username = BuildUsernameFromEmail(email),
                password_hash = _passwordService.HashPassword(request.Password),
                role = GuestRole,
                user_status = ActiveStatus,
                created_at = DateTime.UtcNow,
                is_email_verified = false
            };

            await _users.AddAsync(newUser, ct);
            await _users.SaveChangesAsync(ct);

            return _jwtService.GenerateAccessToken(newUser);
        }

        public async Task<AuthResponse?> LoginGuestAsync(
            LoginGuestRequest request,
            CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _users.GetByEmailAsync(email, ct);

            if (user == null)
                return null;

            if (user.role != GuestRole)
                return null;

            if (user.user_status != ActiveStatus)
                return null;

            if (string.IsNullOrEmpty(user.password_hash) ||
            !_passwordService.VerifyPassword(user.password_hash, request.Password))
                    {
                        return null;
                    }

            user.last_login_at = DateTime.UtcNow;

            await _users.SaveChangesAsync(ct);

            return _jwtService.GenerateAccessToken(user);
        }

        private static string BuildUsernameFromEmail(string email)
        {
            var index = email.IndexOf('@');
            return index > 0 ? email[..index] : email;
        }
    }
}
