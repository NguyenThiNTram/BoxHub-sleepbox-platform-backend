using AutoMapper;
using BoxHub.Application.DTOs.Requests.Users;
using BoxHub.Application.DTOs.Responses.Users;
using BoxHub.Application.Interfaces;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;

namespace BoxHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;

        public UserService(IUserRepository users, IMapper mapper)
        {
            _users = users;
            _mapper = mapper;
        }

        public async Task<UserProfileResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            var profile = await _users.GetProfileAsync(userId, ct);

            return MapToResponse(user, profile);
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

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

            await _users.SaveChangesAsync(ct);

            return MapToResponse(user, profile);
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

        public async Task DeactivateAccountAsync(Guid userId, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(userId, ct);

            if (user == null)
                throw new ApiException(ErrorCodes.UserNotFound, "User not found", 404);

            if (user.user_status == UserStatus.Inactive)
                throw new ApiException(ErrorCodes.UserInactive, "User already inactive", 400);

            user.user_status = UserStatus.Inactive;
            user.deleted_at = DateTime.UtcNow;

            await _users.SaveChangesAsync(ct);
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

            await _users.SaveChangesAsync(ct);
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private UserProfileResponse MapToResponse(user user, user_profile? profile)
        {
            var response = _mapper.Map<UserProfileResponse>(user);

            if (profile != null)
            {
                response.FirstName = profile.first_name;
                response.LastName = profile.last_name;
                response.Gender = profile.gender;
                response.AvatarUrl = profile.avatar_url;

                if (profile.date_of_birth.HasValue)
                    response.DateOfBirth = profile.date_of_birth;
            }

            return response;
        }
    }
}