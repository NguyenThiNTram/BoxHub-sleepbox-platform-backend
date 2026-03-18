using AutoMapper;
using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Admins;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BoxHub.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AdminService(IUserRepository userRepository, IPasswordService passwordService, IUnitOfWork uow, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<CreateAdminResponse> CreateAdminAsync(Guid currentAdminId, CreateAdminRequest request, CancellationToken ct)
        {
            // Validate current admin
            await ValidateAdminAsync(currentAdminId, ct);

            var existedEmail = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existedEmail != null)
                throw new ApiException(ErrorCodes.EmailExists, "Email already exists", 409);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                var users = new user
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

                await _userRepository.AddAsync(users, ct);

                // Create profile
                var profile = new user_profile
                {
                    profile_id = Guid.NewGuid(),
                    user_id = users.user_id,
                    first_name = request.FirstName,
                    last_name = request.LastName,
                    gender = request.Gender,
                    date_of_birth = request.DateOfBirth
                };

                await _userRepository.CreateProfileAsync(profile, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return new CreateAdminResponse
                {
                    UserId = users.user_id,
                    Username = users.username,
                    Email = users.email,
                    Gender = profile.gender,
                    DateOfBirth = profile.date_of_birth,
                    Role = users.role.ToString(),
                    CreatedAt = users.created_at,
                };
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<UserItemResponse> CreateModeratorAsync(Guid currentAdminId, CreateModeratorRequest request, CancellationToken ct)
        {
            await ValidateAdminAsync(currentAdminId, ct);

            var existedEmail = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existedEmail != null)
                throw new ApiException(ErrorCodes.EmailExists, "Email already exists", 409);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                var user = new user
                {
                    user_id = Guid.NewGuid(),
                    username = request.Username,
                    email = request.Email,
                    phone = request.Phone,
                    password_hash = _passwordService.HashPassword(request.Password),
                    role = UserRole.Moderator,
                    user_status = UserStatus.Active,
                    is_email_verified = true,
                    created_at = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user, ct);

                var profile = new user_profile
                {
                    profile_id = Guid.NewGuid(),
                    user_id = user.user_id,
                    first_name = request.FirstName,
                    last_name = request.LastName,
                    gender = request.Gender,
                    date_of_birth = request.DateOfBirth
                };

                await _userRepository.CreateProfileAsync(profile, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return _mapper.Map<UserItemResponse>(user);
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<UserItemResponse> UpdateModeratorAsync(Guid currentAdminId, Guid moderatorId, UpdateModeratorRequest request, CancellationToken ct)
        {
            await ValidateAdminAsync(currentAdminId, ct);

            var user = await _userRepository.GetByIdAsync(moderatorId, ct);
            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            if (user.role != UserRole.Moderator)
                throw new ApiException(ErrorCodes.ValidationFailed, "User is not moderator", 400);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                // Update basic info
                if (!string.IsNullOrWhiteSpace(request.Username))
                    user.username = request.Username;

                if (!string.IsNullOrWhiteSpace(request.Phone))
                    user.phone = request.Phone;

                await _userRepository.UpdateAsync(user, ct);

                // Update profile
                var profile = await _userRepository.GetProfileAsync(user.user_id, ct);

                if (profile != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.FirstName))
                        profile.first_name = request.FirstName;

                    if (!string.IsNullOrWhiteSpace(request.LastName))
                        profile.last_name = request.LastName;

                    await _userRepository.UpdateProfileAsync(profile, ct);
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return _mapper.Map<UserItemResponse>(user);
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<PagedResponse<UserItemResponse>> GetUsersAsync(Guid currentAdminId, GetUsersRequest request, CancellationToken ct)
        {
            await ValidateAdminAsync(currentAdminId, ct);

            var (users, totalCount) = await _userRepository.GetUsersAsync(request, ct);

            var items = _mapper.Map<IEnumerable<UserItemResponse>>(users);

            return new PagedResponse<UserItemResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task SuspendUserAsync(Guid currentAdminId, Guid targetUserId, CancellationToken ct)
        {
            //var currentAdmin = await _userRepository.GetByIdAsync(currentAdminId, ct);
            var admin = await ValidateAdminAsync(currentAdminId, ct);

            // Cannot suspend yourself
            if (currentAdminId == targetUserId)
                throw new ApiException(ErrorCodes.ValidationFailed, "Cannot suspend yourself", 400);


            var targetUser = await _userRepository.GetByIdAsync(targetUserId, ct);
            if (targetUser == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            if (targetUser.role == UserRole.Admin)
                throw new ApiException(ErrorCodes.ValidationFailed, "Cannot suspend admin", 403);

            // Admin can only suspend Moderators
            if (targetUser.role != UserRole.Moderator)
                throw new ApiException(ErrorCodes.ValidationFailed,
                    "Admin can only suspend moderators", 403);

            // Check if already suspended
            if (targetUser.user_status == UserStatus.Suspended)
                throw new ApiException(ErrorCodes.ValidationFailed, "User already suspended", 400);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                // Update user status to SUSPENDED (not INACTIVE)
                var oldStatus = targetUser.user_status;
                targetUser.user_status = UserStatus.Suspended;

                await _userRepository.UpdateAsync(targetUser, ct);

                // Create audit log
                var auditLog = new audit_log
                {
                    audit_id = Guid.NewGuid(),
                    actor_id = admin.user_id,
                    actor_role = admin.role.ToString(),
                    action = "SUSPEND_USER",
                    target_type = "USER",
                    target_id = targetUserId,
                    old_value = JsonSerializer.Serialize(new { user_status = oldStatus }),
                    new_value = JsonSerializer.Serialize(new
                    {
                        user_status = UserStatus.Suspended,
                        suspended_at = DateTime.UtcNow
                    }),
                    note = $"Moderator suspended by Admin {admin.username}",
                    created_at = DateTime.UtcNow
                };

                await _userRepository.AddAuditLogAsync(auditLog, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        

        private async Task<user> ValidateAdminAsync(Guid adminId, CancellationToken ct)
        {
            var admin = await _userRepository.GetByIdAsync(adminId, ct);

            if (admin == null)
                throw new ApiException(ErrorCodes.Unauthorized, "User not found", 401);

            if (admin.role != UserRole.Admin)
                throw new ApiException(ErrorCodes.Unauthorized, "Admin role required", 403);

            if (admin.user_status != UserStatus.Active)
                throw new ApiException(ErrorCodes.UserInactive, "Admin is inactive", 403);

            return admin;
        }
    }
}