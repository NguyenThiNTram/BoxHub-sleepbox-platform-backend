using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Responses.Admins;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Http;

namespace BoxHub.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _uow;

        public AdminService(IUserRepository userRepository, IPasswordService passwordService, IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _uow = uow;
        }

        public async Task<CreateAdminResponse> CreateAdminAsync(Guid currentAdminId, CreateAdminRequest request, CancellationToken ct)
        {
            // Validate current admin
            var currentAdmin = await _userRepository.GetByIdAsync(currentAdminId, ct);
            if (currentAdmin == null)
                throw new ApiException(
                    ErrorCodes.Unauthorized,
                    "User not found",
                    StatusCodes.Status401Unauthorized);

            if (currentAdmin.role != UserRole.Admin)
                    throw new ApiException(
                    ErrorCodes.Unauthorized,
                    "Only admin can create admin",
                    StatusCodes.Status403Forbidden);

            if (currentAdmin.user_status != UserStatus.Active)
                    throw new ApiException(
                    ErrorCodes.UserInactive,
                    "User is inactive",
                    StatusCodes.Status403Forbidden);

            var existedEmail = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existedEmail != null)
                throw new ApiException(
                    ErrorCodes.UsernameExists,
                    "Email already exists",
                    StatusCodes.Status409Conflict);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                // 4. Create user
                var newUser = new user
                {
                    user_id = Guid.NewGuid(),
                    username = request.Username,
                    email = request.Email,
                    phone = request.Phone,
                    password_hash = _passwordService.HashPassword(request.Password),
                    role = UserRole.Admin,
                    user_status = UserStatus.Active,
                    is_email_verified = true,
                    created_at = DateTime.UtcNow
                };

                await _userRepository.AddAsync(newUser, ct);

                // Create profile
                var profile = new user_profile
                {
                    profile_id = Guid.NewGuid(),
                    user_id = newUser.user_id,
                    first_name = request.FirstName,
                    last_name = request.LastName
                };

                await _userRepository.CreateProfileAsync(profile, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return new CreateAdminResponse
                {
                    UserId = newUser.user_id,
                    Username = newUser.username,
                    Email = newUser.email,
                    Role = newUser.role.ToString(),
                    CreatedAt = newUser.created_at,
                };
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        
    }
}