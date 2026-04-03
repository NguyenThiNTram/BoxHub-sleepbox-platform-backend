using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;

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

    Task<int> CountOtpsCreatedSinceAsync(string email, OTPPurpose purpose, DateTimeOffset sinceUtc, CancellationToken ct);

    /// <summary>Bản ghi OTP mới nhất (theo created_at) cho email + mục đích.</summary>
    Task<email_otp?> GetLatestOtpForEmailAsync(string email, OTPPurpose purpose, CancellationToken ct);

    Task<email_otp?> FindValidOtpAsync(string email, string otpCode, OTPPurpose purpose, DateTimeOffset utcNow, CancellationToken ct);

    Task<host_registration_draft?> FindLatestDraftByEmailAsync(string email, CancellationToken ct);

    Task<email_otp?> GetOtpByIdAsync(Guid otpId, bool track, CancellationToken ct);

    Task<host_registration_draft?> GetDraftByOtpIdAsync(Guid otpId, bool track, CancellationToken ct);

    /// <summary>Đã có host_profile khác dùng số CCCD/CMND này (chuẩn hóa UPPER, trim).</summary>
    Task<bool> RepresentativeIdNumberTakenAsync(string normalizedRepresentativeId, CancellationToken ct);

    /// <summary>Đã có host_profile khác dùng mã số thuế này (chuẩn hóa UPPER, trim).</summary>
    Task<bool> TaxCodeTakenAsync(string normalizedTaxCode, CancellationToken ct);

    /// <summary>Đã có brand đang hoạt động trùng tên (chuẩn hóa UPPER, trim).</summary>
    Task<bool> BrandNameTakenAsync(string normalizedBrandName, CancellationToken ct);
}
