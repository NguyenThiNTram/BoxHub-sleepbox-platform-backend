using BoxHub.Application.DTOs.Requests.Users;
using BoxHub.Application.DTOs.Responses.Users;

namespace BoxHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct);

        Task<UserProfileResponse> UpdateUserProfileAsync(
            Guid userId,
            UpdateUserProfileRequest request,
            CancellationToken ct);
    }
}

