using AutoMapper;
using BoxHub.Application.DTOs.Requests.Admins;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Admins;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Results;
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

        public AdminService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<CreateAdminResponse>> CreateAdminAsync(Guid currentAdminId, CreateAdminRequest request, CancellationToken ct)
        {
            var adminResult = await ValidateAdminAsync(currentAdminId, ct);
            if (!adminResult.IsSuccess)
                return Result<CreateAdminResponse>.Failure(adminResult.ErrorCode!, adminResult.ErrorMessage!, adminResult.HttpStatus ?? 400);

            var existedEmail = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existedEmail != null)
                return Result<CreateAdminResponse>.Failure(ErrorCodes.EmailExists, "Email already exists", 409);

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
                    role = UserRole.Admin,
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

                return Result<CreateAdminResponse>.Success(new CreateAdminResponse
                {
                    UserId = user.user_id,
                    Username = user.username,
                    Email = user.email,
                    Gender = profile.gender,
                    DateOfBirth = profile.date_of_birth,
                    Role = user.role.ToString(),
                    CreatedAt = user.created_at
                });
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                return Result<CreateAdminResponse>.Failure(ErrorCodes.ServerError, "Failed to create admin", 500);
            }
        }

        public async Task<Result<UserItemResponse>> CreateModeratorAsync(Guid currentAdminId, CreateModeratorRequest request, CancellationToken ct)
        {
            var adminResult = await ValidateAdminAsync(currentAdminId, ct);
            if (!adminResult.IsSuccess)
                return Result<UserItemResponse>.Failure(adminResult.ErrorCode!, adminResult.ErrorMessage!, adminResult.HttpStatus ?? 400);

            var existedEmail = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existedEmail != null)
                return Result<UserItemResponse>.Failure(ErrorCodes.EmailExists, "Email already exists", 409);

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

                return Result<UserItemResponse>.Success(_mapper.Map<UserItemResponse>(user));
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                return Result<UserItemResponse>.Failure(ErrorCodes.ServerError, "Failed to create moderator", 500);
            }
        }

        public async Task<Result<UserItemResponse>> UpdateModeratorAsync(Guid currentAdminId, Guid moderatorId, UpdateModeratorRequest request, CancellationToken ct)
        {
            var adminResult = await ValidateAdminAsync(currentAdminId, ct);
            if (!adminResult.IsSuccess)
                return Result<UserItemResponse>.Failure(adminResult.ErrorCode!, adminResult.ErrorMessage!, adminResult.HttpStatus ?? 400);

            var user = await _userRepository.GetByIdAsync(moderatorId, ct);
            if (user == null)
                return Result<UserItemResponse>.Failure(ErrorCodes.UserNotFound, "User not found", 404);

            if (user.role != UserRole.Moderator)
                return Result<UserItemResponse>.Failure(ErrorCodes.ValidationFailed, "User is not moderator", 400);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                if (!string.IsNullOrWhiteSpace(request.Username))
                    user.username = request.Username;

                if (!string.IsNullOrWhiteSpace(request.Phone))
                    user.phone = request.Phone;

                await _userRepository.UpdateAsync(user, ct);

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

                return Result<UserItemResponse>.Success(_mapper.Map<UserItemResponse>(user));
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                return Result<UserItemResponse>.Failure(ErrorCodes.ServerError, "Failed to update moderator", 500);
            }
        }

        public async Task<Result<PagedResponse<UserItemResponse>>> GetUsersAsync(Guid currentAdminId, GetUsersRequest request, CancellationToken ct)
        {
            var adminResult = await ValidateAdminAsync(currentAdminId, ct);
            if (!adminResult.IsSuccess)
                return Result<PagedResponse<UserItemResponse>>.Failure(adminResult.ErrorCode!, adminResult.ErrorMessage!, adminResult.HttpStatus ?? 400);

            var (users, totalCount) = await _userRepository.GetUsersAsync(request, ct);

            return Result<PagedResponse<UserItemResponse>>.Success(new PagedResponse<UserItemResponse>
            {
                Items = _mapper.Map<IEnumerable<UserItemResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        public async Task<Result<SimpleMessageResponse>> SuspendUserAsync(Guid currentAdminId, Guid targetUserId, CancellationToken ct)
        {
            var adminResult = await ValidateAdminAsync(currentAdminId, ct);
            if (!adminResult.IsSuccess)
                return Result<SimpleMessageResponse>.Failure(adminResult.ErrorCode!, adminResult.ErrorMessage!, adminResult.HttpStatus ?? 400);

            var admin = adminResult.Value!;

            if (currentAdminId == targetUserId)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Cannot suspend yourself", 400);

            var targetUser = await _userRepository.GetByIdAsync(targetUserId, ct);
            if (targetUser == null)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.UserNotFound, "User not found", 404);

            if (targetUser.role == UserRole.Admin)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Cannot suspend admin", 403);

            if (targetUser.role != UserRole.Moderator)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Only moderators can be suspended", 403);

            if (targetUser.user_status == UserStatus.Suspended)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "User already suspended", 400);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                var oldStatus = targetUser.user_status;
                targetUser.user_status = UserStatus.Suspended;

                await _userRepository.UpdateAsync(targetUser, ct);

                var auditLog = new audit_log
                {
                    audit_id = Guid.NewGuid(),
                    actor_id = admin.user_id,
                    actor_role = admin.role.ToString(),
                    action = "SUSPEND_USER",
                    target_type = "USER",
                    target_id = targetUserId,
                    old_value = JsonSerializer.Serialize(new { user_status = oldStatus }),
                    new_value = JsonSerializer.Serialize(new { user_status = UserStatus.Suspended }),
                    created_at = DateTime.UtcNow
                };

                await _userRepository.AddAuditLogAsync(auditLog, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
                {
                    Success = true,
                    Message = "User suspended successfully"
                });
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ServerError, "Failed to suspend user", 500);
            }
        }

        private async Task<Result<user>> ValidateAdminAsync(Guid adminId, CancellationToken ct)
        {
            var admin = await _userRepository.GetByIdAsync(adminId, ct);

            if (admin == null)
                return Result<user>.Failure(ErrorCodes.Unauthorized, "User not found", 401);

            if (admin.role != UserRole.Admin)
                return Result<user>.Failure(ErrorCodes.Forbidden, "Admin role required", 403);

            if (admin.user_status != UserStatus.Active)
                return Result<user>.Failure(ErrorCodes.UserInactive, "Admin is inactive", 403);

            return Result<user>.Success(admin);
        }
    }
}