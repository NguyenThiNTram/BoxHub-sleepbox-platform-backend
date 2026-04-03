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

        public UserService(
            IUserRepository users,
            IMapper mapper,
            IUnitOfWork uow,
            ICloudinaryService cloudinary)
        {
            _users = users;
            _mapper = mapper;
            _uow = uow;
            _cloudinary = cloudinary;
        }

        public async Task<UserProfileResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "Không tìm thấy người dùng", 404);

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "Không tìm thấy người dùng", 404);

            if (request.Username != null)
            {
                var uname = request.Username.Trim();
                if (string.IsNullOrEmpty(uname))
                    throw new ApiException(ErrorCodes.ValidationFailed, "Tên đăng nhập không hợp lệ.", 400);

                if (!string.Equals(user.username, uname, StringComparison.Ordinal))
                {
                    var byName = await _users.GetByUsernameAsync(uname, ct);
                    if (byName != null && byName.user_id != userId)
                        throw new ApiException(ErrorCodes.UsernameExists, "Tên đăng nhập đã tồn tại.", 409);
                }

                user.username = uname;
            }

            if (request.Phone != null)
                user.phone = Normalize(request.Phone);

            var hasProfileUpdates =
                request.FirstName != null
                || request.LastName != null
                || request.Gender != null
                || request.DateOfBirth.HasValue;

            if (hasProfileUpdates)
            {
                var profile = await _users.GetProfileAsync(userId, ct);
                if (profile == null)
                {
                    profile = CreateProfile(userId);
                    ApplyProfileFields(profile, request);
                    await _users.CreateProfileAsync(profile, ct);
                }
                else
                {
                    ApplyProfileFields(profile, request);
                    await _users.UpdateProfileAsync(profile, ct);
                }
            }

            await _uow.SaveChangesAsync(ct);

            var refreshed = await _users.GetByIdAsync(userId, ct);
            return _mapper.Map<UserProfileResponse>(refreshed!);
        }

        public async Task<UserProfileResponse> UploadAvatarAsync(Guid userId, IFormFile? avatar, CancellationToken ct)
        {
            if (avatar is not { Length: > 0 })
                throw new ApiException(ErrorCodes.ValidationFailed, "Vui lòng gửi file ảnh avatar.", 400);

            var contentType = avatar.ContentType ?? "";
            if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new ApiException(ErrorCodes.ValidationFailed, "Avatar chỉ chấp nhận file ảnh (image/*).", 400);

            var url = await _cloudinary.UploadImageAsync(avatar);
            if (string.IsNullOrWhiteSpace(url))
                throw new ApiException(ErrorCodes.ServerError, "Không upload được ảnh.", 500);

            var user = await _users.GetByIdAsync(userId, ct);
            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            var profile = await _users.GetProfileAsync(userId, ct);
            if (profile == null)
            {
                profile = CreateProfile(userId);
                profile.avatar_url = url;
                profile.updated_at = DateTime.UtcNow;
                await _users.CreateProfileAsync(profile, ct);
            }
            else
            {
                profile.avatar_url = url;
                profile.updated_at = DateTime.UtcNow;
                await _users.UpdateProfileAsync(profile, ct);
            }

            await _uow.SaveChangesAsync(ct);

            var refreshed = await _users.GetByIdAsync(userId, ct);
            return _mapper.Map<UserProfileResponse>(refreshed!);
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

        private static void ApplyProfileFields(user_profile profile, UpdateUserProfileRequest request)
        {
            if (request.FirstName != null)
                profile.first_name = Normalize(request.FirstName);
            if (request.LastName != null)
                profile.last_name = Normalize(request.LastName);
            if (request.Gender != null)
                profile.gender = Normalize(request.Gender);
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
