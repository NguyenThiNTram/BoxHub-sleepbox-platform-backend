using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;

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

        public async Task<AuthResponse> AuthenticateAsync(
            string email,
            string password,
            CancellationToken ct)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ApiException(ErrorCodes.ValidationFailed, "Email and password are required", 400);

            var normalizedEmail = email.Trim().ToLowerInvariant();

            // 2. Get user
            var user = await _users.GetByEmailAsync(normalizedEmail, ct);

            // 3. Prevent user enumeration (same error)
            if (user == null ||
                string.IsNullOrEmpty(user.password_hash) ||
                !_passwordService.VerifyPassword(user.password_hash, password))
            {
                throw new ApiException(ErrorCodes.Unauthorized, "Invalid email or password", 401);
            }

            // 4. Check status
            if (user.user_status != ActiveStatus)
            {
                throw new ApiException(ErrorCodes.UserInactive, "User is inactive", 403);
            }

            // 5. Update last login
            user.last_login_at = DateTime.UtcNow;
            await _users.SaveChangesAsync(ct);

            // 6. Generate token
            return _jwtService.GenerateAccessToken(user);
        }

        public async Task<AuthResponse> RegisterGuestAsync(
            RegisterGuestRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "Email and password are required", 400);
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var existing = await _users.GetByEmailAsync(email, ct);
            if (existing != null)
            {
                throw new ApiException(ErrorCodes.UsernameExists, "Email already exists", 409);
            }

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

        private static string BuildUsernameFromEmail(string email)
        {
            var index = email.IndexOf('@');
            return index > 0 ? email[..index] : email;
        }
    }
}