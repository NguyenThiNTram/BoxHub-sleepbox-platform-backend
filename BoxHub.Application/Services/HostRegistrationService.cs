using BoxHub.Application.Constants;
using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Helpers;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Models.HostRegistration;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Helpers;
using BoxHub.Shared.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace BoxHub.Application.Services;

public sealed class HostRegistrationService : IHostRegistrationService
{
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

    public async Task<Result<RegisterHostDraftResponse>> CreateDraftAsync(string? token, RegisterHostDraftForm form, CancellationToken ct)
    {
        var parsed = _jwt.TryValidateHostRegistrationToken(token ?? "");
        if (parsed == null || !string.Equals(parsed.TokenUse, HostRegistrationTokenUses.EmailVerified, StringComparison.OrdinalIgnoreCase))
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        var emailNorm = NormalizeEmail(parsed.Email);
        if (string.IsNullOrWhiteSpace(emailNorm))
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.ValidationFailed, "Email trong token không hợp lệ.", 400);

        var existingUser = await _users.GetByEmailAsync(emailNorm, ct);
        if (existingUser != null && existingUser.role == UserRole.Host)
            return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.EmailExists, "Email đã được đăng ký làm Host.", 409);

        var latest = await _drafts.FindLatestDraftByEmailAsync(emailNorm, ct);
        if (latest != null && !latest.is_verified)
        {
            return Result<RegisterHostDraftResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Đã tồn tại bản nháp cho email này. Vui lòng cập nhật bản nháp hiện tại.",
                409);
        }

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
            if (string.IsNullOrWhiteSpace(form.Username))
                return Result<RegisterHostDraftResponse>.Failure(ErrorCodes.ValidationFailed, "Thiếu username.", 400);

            username = form.Username.Trim();
            phone = form.Phone?.Trim();
            firstName = form.FirstName?.Trim();
            lastName = form.LastName?.Trim();
        }

        if (form.RepresentativeFrontUrl is { Length: > 0 } front && !IsImageFile(front))
            return Result<RegisterHostDraftResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Ảnh mặt trước CCCD chỉ chấp nhận định dạng file ảnh (image/*).",
                400);

        if (form.RepresentativeBackUrl is { Length: > 0 } back && !IsImageFile(back))
            return Result<RegisterHostDraftResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Ảnh mặt sau CCCD chỉ chấp nhận định dạng file ảnh (image/*).",
                400);

        var empty = new HostDraftPayloadModel
        {
            Email = emailNorm,
            EmailVerifiedAt = DateTime.UtcNow,
            ReviewStatus = "pending"
        };

        var merged = await BuildPayloadFromFormAsync(
            form,
            emailNorm,
            empty,
            username,
            phone,
            firstName,
            lastName,
            ct);

        var draftVal = await ValidateMergedHostDraftAsync(merged, emailNorm, ct);
        if (!draftVal.Ok)
            return Result<RegisterHostDraftResponse>.Failure(draftVal.Code!, draftVal.Message!, draftVal.Status);

        var now = DateTime.UtcNow;
        var draft = new host_registration_draft
        {
            draft_id = Guid.NewGuid(),
            email = emailNorm,
            otp_id = null,
            payload = HostRegistrationJson.SerializePayload(merged),
            is_verified = false,
            expire_at = now.AddDays(30),
            created_at = now,
            updated_at = now,
            phone = merged.Phone?.Trim()
        };

        await _drafts.AddDraftAsync(draft, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterHostDraftResponse>.Success(new RegisterHostDraftResponse
        {
            DraftId = draft.draft_id,
            Message = "Đã tạo bản nháp."
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
        var isEmailVerifiedToken = string.Equals(parsed?.TokenUse, HostRegistrationTokenUses.EmailVerified, StringComparison.OrdinalIgnoreCase);
        var isDraftEditToken = string.Equals(parsed?.TokenUse, HostRegistrationTokenUses.DraftEdit, StringComparison.OrdinalIgnoreCase);
        if (parsed == null || (!isEmailVerifiedToken && !isDraftEditToken))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        var draft = await _drafts.GetDraftByIdAsync(draftId, track: true, ct);
        if (draft == null)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        if (isDraftEditToken && parsed!.SubjectId != draftId)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Token không khớp bản nháp.", 403);

        if (!string.Equals(NormalizeEmail(parsed.Email), draft.email, StringComparison.Ordinal))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.Forbidden, "Email không khớp token.", 403);


        if (draft.is_verified)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftLocked, "Bản nháp đã được duyệt, không thể chỉnh sửa.", 403);

        var existingPayload = HostRegistrationJson.DeserializePayload(draft.payload);
        if (string.Equals(existingPayload.ReviewStatus, "processing", StringComparison.OrdinalIgnoreCase))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.DraftLocked, "Hồ sơ đang được xử lý.", 423);


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

            if (string.IsNullOrWhiteSpace(form.Username))
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Thiếu username.", 400);

            username = form.Username.Trim();
            phone = form.Phone?.Trim();
            firstName = form.FirstName?.Trim();
            lastName = form.LastName?.Trim();
        }

        if (form.RepresentativeFrontUrl is { Length: > 0 } front && !IsImageFile(front))
            return Result<SimpleMessageResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Ảnh mặt trước CCCD chỉ chấp nhận định dạng file ảnh (image/*).",
                400);

        if (form.RepresentativeBackUrl is { Length: > 0 } back && !IsImageFile(back))
            return Result<SimpleMessageResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Ảnh mặt sau CCCD chỉ chấp nhận định dạng file ảnh (image/*).",
                400);

        var merged = await BuildPayloadFromFormAsync(
            form,
            emailNorm,
            existingPayload,
            username,
            phone,
            firstName,
            lastName,
            ct);

        var updateVal = await ValidateMergedHostDraftAsync(merged, emailNorm, ct);
        if (!updateVal.Ok)
            return Result<SimpleMessageResponse>.Failure(updateVal.Code!, updateVal.Message!, updateVal.Status);

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
        var isEmailVerifiedToken = string.Equals(parsed?.TokenUse, HostRegistrationTokenUses.EmailVerified, StringComparison.OrdinalIgnoreCase);
        var isDraftEditToken = string.Equals(parsed?.TokenUse, HostRegistrationTokenUses.DraftEdit, StringComparison.OrdinalIgnoreCase);
        if (parsed == null || (!isEmailVerifiedToken && !isDraftEditToken))
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.TokenInvalid, "Token không hợp lệ.", 401);

        var draft = await _drafts.GetDraftByIdAsync(draftId, track: false, ct);
        if (draft == null)
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.DraftNotFound, "Không tìm thấy bản nháp.", 404);

        if (isDraftEditToken && parsed!.SubjectId != draftId)
            return Result<HostDraftForEditResponse>.Failure(ErrorCodes.Forbidden, "Token không khớp bản nháp.", 403);

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

    private async Task<(bool Ok, string? Code, string? Message, int Status)> ValidateMergedHostDraftAsync(
        HostDraftPayloadModel merged,
        string registrantEmailNorm,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(merged.Username))
            return (false, ErrorCodes.ValidationFailed, "Thiếu username.", 400);

        var uname = merged.Username.Trim();
        var byName = await _users.GetByUsernameAsync(uname, ct);
        if (byName != null
            && !string.Equals(NormalizeEmail(byName.email), registrantEmailNorm, StringComparison.OrdinalIgnoreCase))
            return (false, ErrorCodes.UsernameExists, "Tên đăng nhập đã tồn tại.", 409);

        if (string.IsNullOrWhiteSpace(merged.RepresentativeIdNumber)
            || !Regex.IsMatch(merged.RepresentativeIdNumber.Trim(), @"^\d{12}$"))
            return (false, ErrorCodes.ValidationFailed, "Số CCCD phải gồm đúng 12 chữ số.", 400);

        var rep = merged.RepresentativeIdNumber.Trim();
        if (await _drafts.RepresentativeIdNumberTakenAsync(rep, ct))
            return (false, ErrorCodes.ValidationFailed, "Số CCCD/CMND đã được sử dụng cho tài khoản Host khác.", 409);

        if (string.IsNullOrWhiteSpace(merged.TaxCode)
            || !Regex.IsMatch(merged.TaxCode.Trim(), @"^\d{10}$"))
            return (false, ErrorCodes.ValidationFailed, "Mã số thuế phải gồm đúng 10 chữ số.", 400);

        var tax = merged.TaxCode.Trim();
        if (await _drafts.TaxCodeTakenAsync(tax, ct))
            return (false, ErrorCodes.ValidationFailed, "Mã số thuế đã được sử dụng.", 409);

        if (string.IsNullOrWhiteSpace(merged.BrandName))
            return (false, ErrorCodes.ValidationFailed, "Tên thương hiệu là bắt buộc.", 400);

        if (string.IsNullOrWhiteSpace(merged.BrandAvatarUrl))
            return (false, ErrorCodes.ValidationFailed, "Logo thương hiệu là bắt buộc.", 400);

        var brandNorm = merged.BrandName.Trim().ToUpperInvariant();
        if (await _drafts.BrandNameTakenAsync(brandNorm, ct))
            return (false, ErrorCodes.ValidationFailed, "Tên thương hiệu đã được sử dụng.", 409);

        return (true, null, null, 0);
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

        if (form.CompanyRegistrationFile is { Length: > 0 } cr)
        {
            var url = IsImageFile(cr)
                ? await _cloudinary.UploadImageAsync(cr)
                : await _cloudinary.UploadFileAsync(cr);

            if (!string.IsNullOrWhiteSpace(url))
                UpsertDoc("COMPANY_REGISTRATION", new List<string> { url });
        }

        // --- Images (CCCD mặt trước/mặt sau + brand logo) ---
        var representativeFrontUrl = existingPayload.RepresentativeFrontUrl;
        if (form.RepresentativeFrontUrl is { Length: > 0 })
        {
            var url = await _cloudinary.UploadImageAsync(form.RepresentativeFrontUrl);
            if (!string.IsNullOrWhiteSpace(url))
                representativeFrontUrl = url;
        }
        var representativeBackUrl = existingPayload.RepresentativeBackUrl;
        if (form.RepresentativeBackUrl is { Length: > 0 })
        {
            var url = await _cloudinary.UploadImageAsync(form.RepresentativeBackUrl);
            if (!string.IsNullOrWhiteSpace(url))
                representativeBackUrl = url;
        }

        var brandAvatarUrl = existingPayload.BrandAvatarUrl;
        if (form.BrandAvatar is { Length: > 0 })
        {
            var url = await _cloudinary.UploadImageAsync(form.BrandAvatar);
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
                : existingPayload.PaymentMethod,

            BankName = !string.IsNullOrWhiteSpace(form.BankName) ? form.BankName.Trim() : existingPayload.BankName,
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

    private static bool IsImageFile(IFormFile file)
    {
        if (!string.IsNullOrWhiteSpace(file.ContentType) &&
            file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fileName = file.FileName;
        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(ext))
            return false;

        ext = ext.ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".heic" or ".heif";
    }
}
