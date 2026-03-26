using BoxHub.Application.Constants;
using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Helpers;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Models.HostRegistration;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Domain.Entities;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Helpers;
using BoxHub.Shared.Results;
using Microsoft.Extensions.Configuration;

namespace BoxHub.Application.Services;

public sealed class HostRegistrationService : IHostRegistrationService
{
    private const OTPPurpose OtpPurpose = OTPPurpose.VERIFY_EMAIL;

    private readonly IHostRegistrationRepository _drafts;
    private readonly IUserRepository _users;
    private readonly ICloudinaryService _cloudinary;
    private readonly IEmailService _email;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;
    private readonly IPasswordService _passwordService;
    private readonly IHostRegistrationJobClient _jobs;

    public HostRegistrationService(
        IHostRegistrationRepository drafts,
        IUserRepository users,
        ICloudinaryService cloudinary,
        IEmailService email,
        IJwtService jwt,
        IUnitOfWork uow,
        IConfiguration config,
        IPasswordService passwordService,
        IHostRegistrationJobClient jobs)
    {
        _drafts = drafts;
        _users = users;
        _cloudinary = cloudinary;
        _jwt = jwt;
        _uow = uow;
        _config = config;
        _passwordService = passwordService;
        _jobs = jobs;
        _email = email;
    }

    /// <inheritdoc />
    public async Task<Result<RegisterHostDraftResponse>> RegisterDraftAsync(RegisterHostDraftForm form, CancellationToken ct)
    {
        var email = NormalizeEmail(form.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.ValidationFailed, "Email không hợp lệ.", 400);

        // Validate email format (đảm bảo “email hợp lệ” đúng theo yêu cầu bước 1).
        try
        {
            _ = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.ValidationFailed, "Email không hợp lệ.", 400);
        }

        // Nếu email đã là HOST thì chặn tạo request mới.
        var existingUser = await _users.GetByEmailAsync(email, ct);
        if (existingUser != null && existingUser.role == UserRole.Host)
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.EmailExists, "Email đã được đăng ký làm Host.", 409);

        // Còn bản nháp chưa hoàn tất (chưa duyệt xong / is_verified) — không tạo POST mới (dùng OTP hoặc link sửa).
        var latest = await _drafts.FindLatestDraftByEmailAsync(email, ct);
        if (latest != null && !latest.is_verified)
            return Result<RegisterHostDraftResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Đã tồn tại bản nháp cho email này. Vui lòng xác thực OTP, hoặc dùng liên kết chỉnh sửa nếu được gửi từ moderator.",
                409);

        var rate = await CheckOtpRateLimitsForNewOtpAsync(email, ct);
        if (!rate.Ok)
            return Result<RegisterHostDraftResponse>.Failure(rate.Code!, rate.Message!, rate.Status ?? 429);

        var otpCode = GenerateOtpCode();
        var utcNow = DateTime.UtcNow;

        var otp = new email_otp
        {
            otp_id = Guid.NewGuid(),
            email = email,
            otp_code = otpCode,
            purpose = OtpPurpose,
            expire_at = utcNow.AddMinutes(10),
            is_used = false,
            attempt_count = 0,
            created_at = utcNow
        };

        var draft = new host_registration_draft
        {
            draft_id = Guid.NewGuid(),
            email = email,
            otp_id = otp.otp_id,
            payload = HostRegistrationJson.SerializePayload(new HostDraftPayloadModel
            {
                Email = email,
                ReviewStatus = "otp_pending"
            }),
            is_verified = false,
            expire_at = utcNow.AddDays(30),
            created_at = utcNow,
            updated_at = utcNow
        };

        await _drafts.AddOtpAsync(otp, ct);
        await _drafts.AddDraftAsync(draft, ct);
        await _uow.SaveChangesAsync(ct);

        await SendOtpEmailAsync(email, otpCode, ct);

        return Result<RegisterHostDraftResponse>.Success(new RegisterHostDraftResponse
        {
            DraftId = draft.draft_id,
            Message = "Đã tạo bản nháp. Vui lòng kiểm tra email để lấy mã OTP."
        });
    }

    /// <inheritdoc />
    public async Task<Result<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var code = request.OtpCode.Trim();

        var otpMatch = await _drafts.FindValidOtpAsync(email, code, OtpPurpose, DateTime.UtcNow, ct);
        if (otpMatch == null)
            return Result<VerifyOtpResponse>.Failure(ErrorCodes.OtpInvalid, "Mã OTP không đúng hoặc đã hết hạn.", 400);

        var draft = await _drafts.GetDraftByOtpIdAsync(otpMatch.otp_id, track: true, ct);
        if (draft == null)
            return Result<VerifyOtpResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        var trackedOtp = await _drafts.GetOtpByIdAsync(otpMatch.otp_id, track: true, ct);
        if (trackedOtp == null)
            return Result<VerifyOtpResponse>.Failure(ErrorCodes.OtpInvalid, "OTP không hợp lệ.", 400);

        var payload = HostRegistrationJson.DeserializePayload(draft.payload);
        trackedOtp.is_used = true;
        payload.EmailVerifiedAt = DateTime.UtcNow;
        draft.payload = HostRegistrationJson.SerializePayload(payload);
        draft.updated_at = DateTime.UtcNow;

        await _drafts.UpdateOtpAsync(trackedOtp, ct);
        await _drafts.UpdateDraftAsync(draft, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwt.CreateHostRegistrationToken(
            draft.draft_id,
            draft.email,
            HostRegistrationTokenUses.DraftEdit,
            TimeSpan.FromDays(14));

        return Result<VerifyOtpResponse>.Success(new VerifyOtpResponse
        {
            Success = true,
            Message = "Xác thực email thành công.",
            DraftId = draft.draft_id,
            Token = token
        });
    }

    /// <inheritdoc />
    public async Task<Result<SimpleMessageResponse>> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var utcNow = DateTime.UtcNow;

        var latest = await _drafts.GetLatestOtpForEmailAsync(email, OtpPurpose, ct);
        if (latest != null && (utcNow - latest.created_at).TotalSeconds < 60)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpRateLimited,
                "Vui lòng đợi ít nhất 60 giây trước khi gửi lại OTP.", 429);

        var draft = await _drafts.FindLatestDraftByEmailAsync(email, ct);
        if (draft == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftNotFound, "Không có bản nháp cho email này.", 404);

        if (!draft.otp_id.HasValue)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpInvalid, "Không có OTP để gửi lại.", 400);

        var otp = await _drafts.GetOtpByIdAsync(draft.otp_id.Value, track: false, ct);
        if (otp == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpInvalid, "Không có OTP để gửi lại.", 400);

        // Gửi lại đúng mã nếu chưa dùng và chưa hết hạn (không tạo bản ghi mới — không tăng count 5/10p).
        if (!otp.is_used && otp.expire_at > utcNow)
        {
            await SendOtpEmailAsync(email, otp.otp_code, ct);
            return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
            {
                Success = true,
                Message = "Đã gửi lại mã OTP qua email."
            });
        }

        // Tạo OTP mới — áp dụng giới hạn 5 lần / 10 phút (theo created_at).
        var count = await _drafts.CountOtpsCreatedSinceAsync(email, OtpPurpose, utcNow.AddMinutes(-10), ct);
        if (count >= 5)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpRateLimited,
                "Đã vượt quá 5 lần gửi OTP trong 10 phút.", 429);

        var newOtp = new email_otp
        {
            otp_id = Guid.NewGuid(),
            email = email,
            otp_code = GenerateOtpCode(),
            purpose = OtpPurpose,
            expire_at = utcNow.AddMinutes(10),
            is_used = false,
            attempt_count = 0,
            created_at = utcNow
        };

        await _drafts.AddOtpAsync(newOtp, ct);
        var trackedDraft = await _drafts.GetDraftByIdAsync(draft.draft_id, track: true, ct);
        if (trackedDraft == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);
        trackedDraft.otp_id = newOtp.otp_id;
        trackedDraft.updated_at = utcNow;
        await _drafts.UpdateDraftAsync(trackedDraft, ct);
        await _uow.SaveChangesAsync(ct);

        await SendOtpEmailAsync(email, newOtp.otp_code, ct);
        return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
        {
            Success = true,
            Message = "Đã gửi mã OTP mới qua email."
        });
    }

    /// <inheritdoc />
    public async Task<Result<SimpleMessageResponse>> UpdateDraftAsync(
        Guid draftId,
        string? token,
        RegisterHostDraftForm form,
        CancellationToken ct)
    {
        var parsed = _jwt.TryValidateHostRegistrationToken(token ?? "");
        if (parsed == null || !string.Equals(parsed.TokenUse, HostRegistrationTokenUses.DraftEdit, StringComparison.OrdinalIgnoreCase))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        if (parsed.SubjectId != draftId)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.TokenInvalid, "Token không khớp bản nháp.", 400);

        var draft = await _drafts.GetDraftByIdAsync(draftId, track: true, ct);
        if (draft == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        if (!string.Equals(NormalizeEmail(parsed.Email), draft.email, StringComparison.Ordinal))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Email không khớp token.", 403);

        // Chỉnh sửa chỉ khi chưa được moderator duyệt (sau approve is_verified = true).
        if (draft.is_verified)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftLocked, "Bản nháp đã được duyệt, không thể chỉnh sửa.", 403);

        var existingPayload = HostRegistrationJson.DeserializePayload(draft.payload);
        if (string.Equals(existingPayload.ReviewStatus, "processing", StringComparison.OrdinalIgnoreCase))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftLocked, "Hồ sơ đang được xử lý.", 423);

        // Luồng mới: bắt buộc OTP đã verify trước khi điền thông tin Host.
        if (existingPayload.EmailVerifiedAt == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Chưa xác thực email (OTP).", 403);

        // Lấy user/profile để fill các trường (nếu email đã tồn tại).
        var emailNorm = NormalizeEmail(draft.email);
        var user = await _users.GetByEmailAsync(emailNorm, ct);

        string username;
        string? phone;
        string? firstName;
        string? lastName;

        if (user != null)
        {
            username = user.username;
            phone = user.phone;
            var profile = await _users.GetProfileAsync(user.user_id, ct);
            firstName = profile?.first_name?.Trim();
            lastName = profile?.last_name?.Trim();
        }
        else
        {
            // Trường hợp email chưa có user: cho phép (nếu FE gửi) nhưng vẫn theo nguyên tắc “OTP-first”.
            if (string.IsNullOrWhiteSpace(form.Username))
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Thiếu username.", 400);

            username = form.Username.Trim();
            phone = form.Phone?.Trim();
            firstName = form.FirstName?.Trim();
            lastName = form.LastName?.Trim();

            // Chặn username trùng nếu tạo user mới.
            var uByName = await _users.GetByUsernameAsync(username, ct);
            if (uByName != null && !string.Equals(uByName.email, emailNorm, StringComparison.OrdinalIgnoreCase))
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.UsernameExists, "Tên đăng nhập đã tồn tại.", 409);
        }

        var merged = await BuildPayloadFromFormAsync(
            form,
            emailNorm,
            existingPayload,
            username,
            phone,
            firstName,
            lastName,
            ct);

        merged.ReviewStatus = "pending";
        merged.RejectReason = null;
        merged.DocumentReviews = null;
        merged.ModeratorId = null;
        merged.ModeratorReviewedAt = null;
        merged.EmailVerifiedAt = existingPayload.EmailVerifiedAt;

        draft.phone = merged.Phone?.Trim();
        draft.payload = HostRegistrationJson.SerializePayload(merged);
        draft.updated_at = DateTime.UtcNow;

        await _drafts.UpdateDraftAsync(draft, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
        {
            Success = true,
            Message = "Đã cập nhật bản nháp."
        });
    }

    /// <inheritdoc />
    public async Task<Result<HostDraftForEditResponse>> GetDraftForEditAsync(
        Guid draftId,
        string? token,
        CancellationToken ct)
    {
        var parsed = _jwt.TryValidateHostRegistrationToken(token ?? "");
        if (parsed == null || !string.Equals(parsed.TokenUse, HostRegistrationTokenUses.DraftEdit, StringComparison.OrdinalIgnoreCase))
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        if (parsed.SubjectId != draftId)
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.TokenInvalid, "Token không khớp bản nháp.", 400);

        var draft = await _drafts.GetDraftByIdAsync(draftId, track: false, ct);
        if (draft == null)
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        if (!string.Equals(NormalizeEmail(parsed.Email), draft.email, StringComparison.Ordinal))
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.Forbidden, "Email không khớp token.", 403);

        if (draft.is_verified)
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.DraftLocked, "Bản nháp đã được duyệt.", 403);

        var p = HostRegistrationJson.DeserializePayload(draft.payload);
        if (string.Equals(p.ReviewStatus, "processing", StringComparison.OrdinalIgnoreCase))
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.DraftLocked, "Hồ sơ đang được xử lý.", 423);

        var docs = p.Documents.Select(d => new HostDraftDocumentItemResponse
        {
            DocumentType = d.DocumentType,
            Attachments = new List<string>(d.Attachments)
        }).ToList();

        return Result<HostDraftForEditResponse>.Success(new HostDraftForEditResponse
        {
            DraftId = draft.draft_id,
            Username = p.Username,
            Email = p.Email,
            Phone = p.Phone,
            FirstName = p.FirstName,
            LastName = p.LastName,
            RepresentativeIdName = p.RepresentativeIdName,
            RepresentativeIdNumber = p.RepresentativeIdNumber,
            TaxCode = p.TaxCode,
            RepresentativeFrontUrl = p.RepresentativeFrontUrl,
            RepresentativeBackUrl = p.RepresentativeBackUrl,
            BrandName = p.BrandName,
            BrandAvatarUrl = p.BrandAvatarUrl,
            BusinessName = p.BusinessName,
            AddressDistrict = p.AddressDistrict,
            AddressWard = p.AddressWard,
            AddressDetail = p.AddressDetail,
            PaymentMethod = p.PaymentMethod,
            BankName = p.BankName,
            BankBranch = p.BankBranch,
            AccountNumber = p.AccountNumber,
            AccountName = p.AccountName,
            Documents = docs,
            ReviewStatus = p.ReviewStatus,
            EmailVerifiedAt = p.EmailVerifiedAt
        });
    }

    /// <inheritdoc />
    public async Task<Result<SimpleMessageResponse>> SetPasswordAsync(string? token, HostSetPasswordRequest request, CancellationToken ct)
    {
        var parsed = _jwt.TryValidateHostRegistrationToken(token ?? "");
        if (parsed == null || !string.Equals(parsed.TokenUse, HostRegistrationTokenUses.SetPassword, StringComparison.OrdinalIgnoreCase))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        if (request.NewPassword != request.ConfirmPassword)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Mật khẩu xác nhận không khớp.", 400);

        var user = await _users.GetByIdAsync(parsed.SubjectId, ct);
        if (user == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.UserNotFound, "Không tìm thấy người dùng.", 404);

        if (!string.Equals(user.email, parsed.Email, StringComparison.OrdinalIgnoreCase))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Token không khớp tài khoản.", 403);

        user.password_hash = _passwordService.HashPassword(request.NewPassword);
        await _users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
        {
            Success = true,
            Message = "Đã đặt mật khẩu thành công."
        });
    }

    /// <inheritdoc />
    public async Task<Result<ModeratorHostDraftListResponse>> ListDraftsForModeratorAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var filter = string.IsNullOrWhiteSpace(status) ? null : status.Trim().ToLowerInvariant();
        var (items, total) = await _drafts.GetDraftsPagedAsync(page, pageSize, filter, ct);

        var list = new List<ModeratorHostDraftSummaryResponse>();
        foreach (var d in items)
        {
            var p = HostRegistrationJson.DeserializePayload(d.payload);
            list.Add(new ModeratorHostDraftSummaryResponse
            {
                DraftId = d.draft_id,
                Email = d.email,
                Phone = d.phone,
                ReviewStatus = p.ReviewStatus,
                CreatedAt = d.created_at,
                EmailVerifiedAt = p.EmailVerifiedAt
            });
        }

        return Result<ModeratorHostDraftListResponse>.Success(new ModeratorHostDraftListResponse
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = list
        });
    }

    /// <inheritdoc />
    public async Task<Result<ModeratorHostDraftDetailResponse>> GetDraftDetailForModeratorAsync(Guid draftId, CancellationToken ct)
    {
        var d = await _drafts.GetDraftByIdAsync(draftId, track: false, ct);
        if (d == null)
            return Result<ModeratorHostDraftDetailResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        var payload = HostRegistrationJson.DeserializePayload(d.payload);
        return Result<ModeratorHostDraftDetailResponse>.Success(new ModeratorHostDraftDetailResponse
        {
            DraftId = d.draft_id,
            Email = d.email,
            Phone = d.phone,
            IsVerified = d.is_verified,
            ExpireAt = d.expire_at,
            CreatedAt = d.created_at,
            UpdatedAt = d.updated_at,
            Payload = payload
        });
    }

    /// <inheritdoc />
    public async Task<Result<SimpleMessageResponse>> ModeratorReviewAsync(
        Guid draftId,
        Guid moderatorId,
        ModeratorReviewHostDraftRequest request,
        CancellationToken ct)
    {
        var action = request.Action.Trim().ToLowerInvariant();
        var draft = await _drafts.GetDraftByIdAsync(draftId, track: true, ct);
        if (draft == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        var payload = HostRegistrationJson.DeserializePayload(draft.payload);

        if (string.Equals(action, "reject", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.RejectReason))
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Vui lòng nhập lý do từ chối.", 400);

            payload.ReviewStatus = "rejected";
            payload.RejectReason = request.RejectReason.Trim();
            payload.ModeratorId = moderatorId;
            payload.ModeratorReviewedAt = DateTime.UtcNow;
            if (request.DocumentReviews != null)
            {
                payload.DocumentReviews = request.DocumentReviews.Select(x => new HostDraftDocumentReviewModel
                {
                    DocumentType = x.DocumentType,
                    Status = x.Status,
                    RejectReason = x.RejectReason
                }).ToList();
            }

            draft.payload = HostRegistrationJson.SerializePayload(payload);
            draft.updated_at = DateTime.UtcNow;
            await _drafts.UpdateDraftAsync(draft, ct);
            await _uow.SaveChangesAsync(ct);

            var token = _jwt.CreateHostRegistrationToken(
                draft.draft_id,
                draft.email,
                HostRegistrationTokenUses.DraftEdit,
                TimeSpan.FromDays(14));

            var editLink = BuildEditDraftLink(token, draft.draft_id);
            var body =
                $"<p>Hồ sơ đăng ký Host bị từ chối.</p><p>Lý do: {System.Net.WebUtility.HtmlEncode(payload.RejectReason)}</p>" +
                $"<p>Vui lòng <a href=\"{editLink}\">chỉnh sửa và gửi lại</a> (liên kết hiệu lực 14 ngày).</p>";
            await _email.SendEmailAsync(new MailData
            {
                EmailToId = draft.email,
                EmailToName = payload.Username,
                EmailSubject = "BoxHub — Hồ sơ Host cần chỉnh sửa",
                EmailBody = body
            });

            return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
            {
                Success = true,
                Message = "Đã từ chối và gửi email cho ứng viên."
            });
        }

        if (string.Equals(action, "approve", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(payload.ReviewStatus, "pending", StringComparison.OrdinalIgnoreCase))
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Chỉ có thể duyệt hồ sơ đang ở trạng thái chờ.", 409);

            if (payload.EmailVerifiedAt == null)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Ứng viên chưa xác thực email (OTP).", 409);

            payload.ReviewStatus = "processing";
            payload.ModeratorId = moderatorId;
            payload.ModeratorReviewedAt = DateTime.UtcNow;
            if (request.DocumentReviews != null)
            {
                payload.DocumentReviews = request.DocumentReviews.Select(x => new HostDraftDocumentReviewModel
                {
                    DocumentType = x.DocumentType,
                    Status = x.Status,
                    RejectReason = x.RejectReason
                }).ToList();
            }

            draft.payload = HostRegistrationJson.SerializePayload(payload);
            draft.updated_at = DateTime.UtcNow;
            await _drafts.UpdateDraftAsync(draft, ct);
            await _uow.SaveChangesAsync(ct);

            _jobs.EnqueueCreateHostAccount(draftId, moderatorId);

            return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
            {
                Success = true,
                Message = "Đã chấp nhận hồ sơ. Hệ thống đang tạo tài khoản Host."
            });
        }

        return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "action phải là approve hoặc reject.", 400);
    }

    // --- helpers ---

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string GenerateOtpCode() => Random.Shared.Next(100000, 1_000_000).ToString();

    private async Task SendOtpEmailAsync(string email, string code, CancellationToken ct)
    {
        _ = ct;
        await _email.SendEmailAsync(new MailData
        {
            EmailToId = email,
            EmailToName = email,
            EmailSubject = "BoxHub — Mã OTP đăng ký Host",
            EmailBody = $"<p>Mã OTP của bạn: <strong>{code}</strong> (hiệu lực 10 phút).</p>"
        });
    }

    private async Task<HostDraftPayloadModel> BuildPayloadFromFormAsync(
        RegisterHostDraftForm form,
        string emailNorm,
        HostDraftPayloadModel existingPayload,
        string username,
        string? phone,
        string? firstName,
        string? lastName,
        CancellationToken ct)
    {
        var docs = new List<HostDraftDocumentModel>();
        if (existingPayload.Documents != null)
        {
            docs.AddRange(existingPayload.Documents.Select(d => new HostDraftDocumentModel
            {
                DocumentType = d.DocumentType,
                Attachments = new List<string>(d.Attachments)
            }));
        }

        void UpsertDoc(string type, List<string> urls)
        {
            var existing = docs.FirstOrDefault(x => string.Equals(x.DocumentType, type, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                foreach (var u in urls)
                    if (!existing.Attachments.Contains(u))
                        existing.Attachments.Add(u);
            }
            else
                docs.Add(new HostDraftDocumentModel { DocumentType = type, Attachments = urls });
        }

        // --- Raw documents (mỗi loại 1 file trong payload hiện tại) ---
        if (form.BusinessLicenseFile is { Length: > 0 } bl)
        {
            var url = await _cloudinary.UploadDocumentAsync(bl);
            if (!string.IsNullOrWhiteSpace(url))
                UpsertDoc("BUSINESS_LICENSE", new List<string> { url });
        }

        if (form.TaxCertificateFile is { Length: > 0 } tc)
        {
            var url = await _cloudinary.UploadDocumentAsync(tc);
            if (!string.IsNullOrWhiteSpace(url))
                UpsertDoc("TAX_CERTIFICATE", new List<string> { url });
        }

        if (form.CompanyRegistrationFile is { Length: > 0 } cr)
        {
            var url = await _cloudinary.UploadDocumentAsync(cr);
            if (!string.IsNullOrWhiteSpace(url))
                UpsertDoc("COMPANY_REGISTRATION", new List<string> { url });
        }

        if (form.PcccFile is { Length: > 0 } pccc)
        {
            var url = await _cloudinary.UploadDocumentAsync(pccc);
            if (!string.IsNullOrWhiteSpace(url))
                UpsertDoc("PCCC", new List<string> { url });
        }

        // --- Images (CCCD mặt trước/mặt sau + brand logo) ---
        var representativeFrontUrl = existingPayload.RepresentativeFrontUrl;
        if (form.RepresentativeFrontUrl is { Length: > 0 })
        {
            var url = await _cloudinary.UploadFileAsync(form.RepresentativeFrontUrl);
            if (!string.IsNullOrWhiteSpace(url))
                representativeFrontUrl = url;
        }
        var representativeBackUrl = existingPayload.RepresentativeBackUrl;
        if (form.RepresentativeBackUrl is { Length: > 0 })
        {
            var url = await _cloudinary.UploadFileAsync(form.RepresentativeBackUrl);
            if (!string.IsNullOrWhiteSpace(url))
                representativeBackUrl = url;
        }

        var brandAvatarUrl = existingPayload.BrandAvatarUrl;
        if (form.BrandAvatar is { Length: > 0 })
        {
            var url = await _cloudinary.UploadFileAsync(form.BrandAvatar);
            if (!string.IsNullOrWhiteSpace(url))
                brandAvatarUrl = url;
        }

        return new HostDraftPayloadModel
        {
            Email = emailNorm,

            Username = username,
            Phone = phone,
            FirstName = firstName,
            LastName = lastName,

            RepresentativeIdName = !string.IsNullOrWhiteSpace(form.RepresentativeIdName)
                ? form.RepresentativeIdName.Trim()
                : existingPayload.RepresentativeIdName,
            RepresentativeIdNumber = !string.IsNullOrWhiteSpace(form.RepresentativeIdNumber)
                ? form.RepresentativeIdNumber.Trim()
                : existingPayload.RepresentativeIdNumber,

            RepresentativeFrontUrl = representativeFrontUrl,
            RepresentativeBackUrl = representativeBackUrl,

            TaxCode = !string.IsNullOrWhiteSpace(form.TaxCode) ? form.TaxCode.Trim() : existingPayload.TaxCode,
            BusinessName = !string.IsNullOrWhiteSpace(form.BusinessName)
                ? form.BusinessName.Trim()
                : existingPayload.BusinessName,

            AddressDistrict = !string.IsNullOrWhiteSpace(form.AddressDistrict)
                ? form.AddressDistrict.Trim()
                : existingPayload.AddressDistrict,
            AddressWard = !string.IsNullOrWhiteSpace(form.AddressWard)
                ? form.AddressWard.Trim()
                : existingPayload.AddressWard,
            AddressDetail = !string.IsNullOrWhiteSpace(form.AddressDetail)
                ? form.AddressDetail.Trim()
                : existingPayload.AddressDetail,

            BrandName = !string.IsNullOrWhiteSpace(form.BrandName) ? form.BrandName.Trim() : existingPayload.BrandName,
            BrandAvatarUrl = brandAvatarUrl,

            PaymentMethod = !string.IsNullOrWhiteSpace(form.PaymentMethod)
                ? form.PaymentMethod.Trim()
                : (form.IsPaymentAtBase == true ? "PAY_AT_BASE" : "BANK_TRANSFER"),

            BankName = !string.IsNullOrWhiteSpace(form.BankName) ? form.BankName.Trim() : existingPayload.BankName,
            BankBranch = !string.IsNullOrWhiteSpace(form.BankBranch) ? form.BankBranch.Trim() : existingPayload.BankBranch,
            AccountNumber = !string.IsNullOrWhiteSpace(form.AccountNumber) ? form.AccountNumber.Trim() : existingPayload.AccountNumber,
            AccountName = !string.IsNullOrWhiteSpace(form.AccountName) ? form.AccountName.Trim() : existingPayload.AccountName,

            Documents = docs,

            // Review fields sẽ được UpdateDraftAsync override.
            ReviewStatus = existingPayload.ReviewStatus,
            RejectReason = existingPayload.RejectReason,
            DocumentReviews = existingPayload.DocumentReviews,
            ModeratorReviewedAt = existingPayload.ModeratorReviewedAt,
            ModeratorId = existingPayload.ModeratorId,
            EmailVerifiedAt = existingPayload.EmailVerifiedAt
        };
    }

    /// <summary>Đăng ký draft: luôn tạo bản ghi OTP mới — kiểm tra 60s + tối đa 5 bản ghi / 10 phút.</summary>
    private async Task<(bool Ok, string? Code, string? Message, int? Status)> CheckOtpRateLimitsForNewOtpAsync(
        string email,
        CancellationToken ct)
    {
        var utcNow = DateTime.UtcNow;
        var windowStart = utcNow.AddMinutes(-10);

        var latest = await _drafts.GetLatestOtpForEmailAsync(email, OtpPurpose, ct);
        if (latest != null && (utcNow - latest.created_at).TotalSeconds < 60)
            return (false, ErrorCodes.OtpRateLimited, "Vui lòng đợi ít nhất 60 giây trước khi gửi OTP.", 429);

        var count = await _drafts.CountOtpsCreatedSinceAsync(email, OtpPurpose, windowStart, ct);
        if (count >= 5)
            return (false, ErrorCodes.OtpRateLimited, "Đã vượt quá 5 lần gửi OTP trong 10 phút.", 429);

        _ = ct;
        return (true, null, null, null);
    }

    /// <summary>
    /// Tạo URL cho FE: nên có cả draftId (route) và token (query/header gọi API).
    /// Placeholder: {draftId}, {token} — token luôn EscapeDataString.
    /// </summary>
    private string BuildEditDraftLink(string token, Guid draftId)
    {
        var template = _config["HostRegistration:EditDraftUrlTemplate"];
        if (string.IsNullOrWhiteSpace(template))
            return token;

        var escaped = Uri.EscapeDataString(token);
        if (template.Contains("{token}", StringComparison.Ordinal) ||
            template.Contains("{draftId}", StringComparison.OrdinalIgnoreCase))
        {
            return template
                .Replace("{draftId}", draftId.ToString(), StringComparison.OrdinalIgnoreCase)
                .Replace("{token}", escaped, StringComparison.Ordinal);
        }

        return $"{template.TrimEnd('/')}?draftId={draftId}&token={escaped}";
    }
}
