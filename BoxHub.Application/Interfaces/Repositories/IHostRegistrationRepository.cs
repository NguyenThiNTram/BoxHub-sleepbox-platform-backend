using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Domain.Entities;

namespace BoxHub.Application.Interfaces.Repositories;

public interface IHostRegistrationRepository
{
    //get host_profile by host_id, return null if not found
    Task<host_profile?> GetByIdAsync(Guid hostId, CancellationToken ct);
    //-----------------------------------------------------------------------------------------------
    Task<host_registration_draft?> GetDraftByIdAsync(Guid draftId, bool track, CancellationToken ct);

    Task AddDraftAsync(host_registration_draft draft, CancellationToken ct);
    Task AddOtpAsync(email_otp otp, CancellationToken ct);
    Task UpdateDraftAsync(host_registration_draft draft, CancellationToken ct);
    Task UpdateOtpAsync(email_otp otp, CancellationToken ct);

    Task<(List<host_registration_draft> Items, int TotalCount)> GetDraftsPagedAsync(
        int page,
        int pageSize,
        string? reviewStatusFilter,
        CancellationToken ct);

    Task<int> CountOtpsCreatedSinceAsync(string email, OTPPurpose purpose, DateTime sinceUtc, CancellationToken ct);

    /// <summary>Bản ghi OTP mới nhất (theo created_at) cho email + mục đích.</summary>
    Task<email_otp?> GetLatestOtpForEmailAsync(string email, OTPPurpose purpose, CancellationToken ct);

    Task<email_otp?> FindValidOtpAsync(string email, string otpCode, OTPPurpose purpose, DateTime utcNow, CancellationToken ct);

    Task<host_registration_draft?> FindLatestDraftByEmailAsync(string email, CancellationToken ct);

    Task<email_otp?> GetOtpByIdAsync(Guid otpId, bool track, CancellationToken ct);

    Task<host_registration_draft?> GetDraftByOtpIdAsync(Guid otpId, bool track, CancellationToken ct);
}
