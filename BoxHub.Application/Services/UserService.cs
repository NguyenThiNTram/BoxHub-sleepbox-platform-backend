using AutoMapper;
using BoxHub.Application.DTOs.Requests.Users;
using BoxHub.Application.DTOs.Responses.Users;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Http;

namespace BoxHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly ICloudinaryService _cloudinary;
        private readonly IPasswordService _passwordService;

        public UserService(
            IUserRepository users,
            IMapper mapper,
            IUnitOfWork uow,
            ICloudinaryService cloudinary,
            IPasswordService passwordService)
        {
            _users = users;
            _mapper = mapper;
            _uow = uow;
            _cloudinary = cloudinary;
            _passwordService = passwordService;
        }

        public async Task<UserProfileResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            var profile = await _users.GetProfileAsync(userId, ct);

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            await _uow.BeginTransactionAsync(ct);

            try
            {
                UpdateUser(user, request);

                var profile = await _users.GetProfileAsync(userId, ct);

                if (profile == null)
                {
                    profile = CreateProfile(userId);
                    UpdateProfile(profile, request);

                    await _users.CreateProfileAsync(profile, ct);
                }
                else
                {
                    UpdateProfile(profile, request);
                    await _users.UpdateProfileAsync(profile, ct);
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                return _mapper.Map<UserProfileResponse>(user);
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        public async Task SoftDeleteAccountAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            if (user.user_status == UserStatus.Inactive)
                throw new ApiException(ErrorCodes.UserInactive, "User already inactive", 400);

            user.user_status = UserStatus.Inactive;
            user.deleted_at = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);
        }

        public async Task ReactivateAccountAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            if (user.user_status == UserStatus.Active)
                throw new ApiException(ErrorCodes.ValidationFailed, "User already active", 400);

            user.user_status = UserStatus.Active;
            user.deleted_at = null;

            await _uow.SaveChangesAsync(ct);
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "Mật khẩu không được để trống", 400);
            }

            var user = await _users.GetByIdAsync(userId, ct);
            if (user == null)
            {
                throw new ApiException(ErrorCodes.UserNotFound, "Người dùng không tồn tại", 404);
            }

            if (!_passwordService.VerifyPassword(user.password_hash, request.CurrentPassword))
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "Mật khẩu hiện tại không đúng", 400);
            }

            user.password_hash = _passwordService.HashPassword(request.NewPassword);

            await _users.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private static void UpdateUser(user user, UpdateUserProfileRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Username))
                user.username = request.Username.Trim();

            user.phone = Normalize(request.Phone);
        }

        private static void UpdateProfile(user_profile profile, UpdateUserProfileRequest request)
        {
            profile.first_name = Normalize(request.FirstName);
            profile.last_name = Normalize(request.LastName);
            profile.gender = Normalize(request.Gender);
            profile.avatar_url = Normalize(request.AvatarUrl);

            if (request.DateOfBirth.HasValue)
                profile.date_of_birth = request.DateOfBirth;

            profile.updated_at = DateTime.UtcNow;
        }

        private static user_profile CreateProfile(Guid userId)
        {
            return new user_profile
            {
                profile_id = Guid.NewGuid(),
                user_id = userId
            };
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}