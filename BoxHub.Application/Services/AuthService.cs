using BoxHub.Application.DTOs.Requests.Auths;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces;
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
        private readonly IUnitOfWork _uow;
        private readonly IOtpService _otpService;

        public AuthService(IUserRepository users, IPasswordService passwordService, IJwtService jwtService, IUnitOfWork uow, IOtpService otpService)
        {
            _users = users;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _uow = uow;
            _otpService = otpService;
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

            await _users.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

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
            await _uow.SaveChangesAsync(ct);

            return _jwtService.GenerateAccessToken(newUser);
        }

        public Task LogoutAsync(CancellationToken ct)
        {
            _ = ct;
            return Task.CompletedTask;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
            if (user == null)
            {
                // To prevent email enumeration, we just return successfully
                return;
            }

            var sendOtpRequest = new BoxHub.Application.DTOs.Requests.Otp.SendOtpRequest
            {
                Email = request.Email,
                Purpose = OTPPurpose.FORGOT_PASSWORD
            };

            var result = await _otpService.SendAsync(sendOtpRequest, ct);
            if (!result.IsSuccess)
            {
                throw new ApiException(result.ErrorCode ?? ErrorCodes.ServerError, result.ErrorMessage ?? "Failed to send OTP", result.HttpStatus ?? 500);
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "All fields are required", 400);
            }

            var verifyOtpRequest = new BoxHub.Application.DTOs.Requests.Otp.VerifyOtpGenericRequest
            {
                Email = request.Email,
                OtpCode = request.OtpCode,
                Purpose = OTPPurpose.FORGOT_PASSWORD
            };

            var verifyResult = await _otpService.VerifyAsync(verifyOtpRequest, ct);
            if (!verifyResult.IsSuccess)
            {
                throw new ApiException(verifyResult.ErrorCode ?? ErrorCodes.ValidationFailed, verifyResult.ErrorMessage ?? "Invalid OTP", verifyResult.HttpStatus ?? 400);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(normalizedEmail, ct);
            if (user == null)
            {
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);
            }

            user.password_hash = _passwordService.HashPassword(request.NewPassword);
            
            await _users.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private static string BuildUsernameFromEmail(string email)
        {
            var index = email.IndexOf('@');
            return index > 0 ? email[..index] : email;
        }
    }
}