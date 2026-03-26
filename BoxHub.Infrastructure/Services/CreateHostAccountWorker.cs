using System.Text.Json;
using BoxHub.Application.Helpers;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Models.HostRegistration;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Data;
using BoxHub.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BoxHub.Domain.Entities;
using BoxHub.Application.Constants;

namespace BoxHub.Infrastructure.Services;

/// <summary>
/// Tạo User Host + profile + tài liệu sau khi moderator approve (transaction DB).
/// </summary>
public sealed class CreateHostAccountWorker : ICreateHostAccountWorker
{
    private readonly BoxHubDbContext _db;
    private readonly IUserRepository _users;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public CreateHostAccountWorker(
        BoxHubDbContext db,
        IUserRepository users,
        IPasswordService passwordService,
        IJwtService jwt,
        IEmailService email,
        IConfiguration config)
    {
        _db = db;
        _users = users;
        _passwordService = passwordService;
        _jwt = jwt;
        _email = email;
        _config = config;
    }

    public async Task ExecuteAsync(Guid draftId, Guid moderatorId, CancellationToken ct)
    {
        await using var trx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var draft = await _db.host_registration_drafts
                .FirstOrDefaultAsync(d => d.draft_id == draftId, ct);

            if (draft == null)
                return;

            var payload = HostRegistrationJson.DeserializePayload(draft.payload);

            // Đã xử lý xong (idempotent cho Hangfire retry).
            if (string.Equals(payload.ReviewStatus, "approved", StringComparison.OrdinalIgnoreCase) && draft.is_verified)
            {
                await trx.CommitAsync(ct);
                return;
            }

            if (!string.Equals(payload.ReviewStatus, "processing", StringComparison.OrdinalIgnoreCase))
                return;

            if (payload.EmailVerifiedAt == null)
                return;

            var emailNorm = payload.Email.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;

            // 1) Tạo/update user (nếu user đã tồn tại thì chỉ set role Host + email verified)
            var existingUser = await _users.GetByEmailAsync(emailNorm, ct);
            user hostUser;
            if (existingUser != null)
            {
                hostUser = existingUser;
                hostUser.role = UserRole.Host;
                hostUser.user_status = UserStatus.Active;
                hostUser.is_email_verified = true;
                hostUser.email_verified_at = now;
                await _users.UpdateAsync(hostUser, ct);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(payload.Username))
                    return;

                var newUser = new user
                {
                    user_id = Guid.NewGuid(),
                    username = payload.Username.Trim(),
                    email = emailNorm,
                    phone = payload.Phone,
                    role = UserRole.Host,
                    user_status = UserStatus.Active,
                    is_email_verified = true,
                    email_verified_at = now,
                    // users.password_hash là NOT NULL, nên lưu hash tạm cho tới khi user đặt mật khẩu thật.
                    password_hash = _passwordService.HashPassword($"{Guid.NewGuid():N}#Temp1"),
                    created_at = now
                };

                await _users.AddAsync(newUser, ct);
                hostUser = newUser;
            }

            // 2) user_profile
            var existingProfile = await _users.GetProfileAsync(hostUser.user_id, ct);
            if (existingProfile != null)
            {
                existingProfile.first_name = payload.FirstName;
                existingProfile.last_name = payload.LastName;
                existingProfile.updated_at = now;
                await _users.UpdateProfileAsync(existingProfile, ct);
            }
            else
            {
                var profile = new user_profile
                {
                    profile_id = Guid.NewGuid(),
                    user_id = hostUser.user_id,
                    first_name = payload.FirstName,
                    last_name = payload.LastName,
                    gender = null,
                    date_of_birth = null,
                    updated_at = now
                };
                await _users.CreateProfileAsync(profile, ct);
            }

            // 3) host_profile (upsert theo user_id)
            var hostProfile = await _db.host_profiles
                .FirstOrDefaultAsync(h => h.user_id == hostUser.user_id, ct);

            if (hostProfile == null)
            {
                hostProfile = new host_profile
                {
                    host_id = Guid.NewGuid(),
                    user_id = hostUser.user_id,
                    representative_id_name = payload.RepresentativeIdName,
                    representative_id_number = payload.RepresentativeIdNumber,
                    representative_front_url = payload.RepresentativeFrontUrl,
                    representative_back_url = payload.RepresentativeBackUrl,
                    tax_code = payload.TaxCode,
                    business_name = payload.BusinessName,
                    address_district = payload.AddressDistrict,
                    address_ward = payload.AddressWard,
                    address_detail = payload.AddressDetail,
                    verified_status = "APPROVED",
                    verified_at = now,
                    verified_by = moderatorId,
                    created_at = now,
                    updated_at = now
                };
                await _db.host_profiles.AddAsync(hostProfile, ct);
            }
            else
            {
                hostProfile.representative_id_name = payload.RepresentativeIdName;
                hostProfile.representative_id_number = payload.RepresentativeIdNumber;
                hostProfile.representative_front_url = payload.RepresentativeFrontUrl;
                hostProfile.representative_back_url = payload.RepresentativeBackUrl;
                hostProfile.tax_code = payload.TaxCode;
                hostProfile.business_name = payload.BusinessName;
                hostProfile.address_district = payload.AddressDistrict;
                hostProfile.address_ward = payload.AddressWard;
                hostProfile.address_detail = payload.AddressDetail;
                hostProfile.reject_reason = null;
                hostProfile.verified_status = "APPROVED";
                hostProfile.verified_at = now;
                hostProfile.verified_by = moderatorId;
                hostProfile.updated_at = now;
                _db.host_profiles.Update(hostProfile);
            }

            var reviewMap = payload.DocumentReviews?.ToDictionary(
                x => x.DocumentType.Trim().ToUpperInvariant(),
                x => x,
                StringComparer.OrdinalIgnoreCase);

            foreach (var doc in payload.Documents)
            {
                var key = doc.DocumentType.Trim().ToUpperInvariant();
                var rev = reviewMap != null && reviewMap.TryGetValue(key, out var r) ? r : null;
                var status = rev != null && string.Equals(rev.Status, "REJECTED", StringComparison.OrdinalIgnoreCase)
                    ? "REJECTED"
                    : "APPROVED";
                var docTypeNorm = doc.DocumentType.Trim().ToUpperInvariant();

                var existingDoc = await _db.host_documents
                    .FirstOrDefaultAsync(x =>
                        x.host_id == hostProfile.host_id
                        && x.document_type.ToUpper() == docTypeNorm
                        && x.version == 1, ct);

                if (existingDoc == null)
                {
                    var hostDoc = new host_document
                    {
                        document_id = Guid.NewGuid(),
                        host_id = hostProfile.host_id,
                        document_type = doc.DocumentType,
                        version = 1,
                        attachments = JsonSerializer.Serialize(doc.Attachments),
                        document_status = status,
                        reviewed_by = moderatorId,
                        reviewed_at = now,
                        reject_reason = rev?.RejectReason,
                        created_at = now,
                        updated_at = now
                    };
                    await _db.host_documents.AddAsync(hostDoc, ct);
                }
                else
                {
                    existingDoc.attachments = JsonSerializer.Serialize(doc.Attachments);
                    existingDoc.document_status = status;
                    existingDoc.reviewed_by = moderatorId;
                    existingDoc.reviewed_at = now;
                    existingDoc.reject_reason = rev?.RejectReason;
                    existingDoc.updated_at = now;
                    _db.host_documents.Update(existingDoc);
                }
            }

            // 4) Brand (upsert theo host_id + brand_name)
            if (!string.IsNullOrWhiteSpace(payload.BrandName))
            {
                var brandNameNorm = payload.BrandName.Trim().ToUpperInvariant();
                var existingBrand = await _db.brands
                    .FirstOrDefaultAsync(b => b.host_id == hostProfile.host_id
                                              && b.brand_name.ToUpper() == brandNameNorm, ct);

                if (existingBrand == null)
                {
                    var brand = new brand
                    {
                        brand_id = Guid.NewGuid(),
                        host_id = hostProfile.host_id,
                        brand_name = payload.BrandName.Trim(),
                        brand_avatar = payload.BrandAvatarUrl,
                        updated_at = now
                    };
                    await _db.brands.AddAsync(brand, ct);
                }
                else
                {
                    existingBrand.brand_avatar = payload.BrandAvatarUrl;
                    existingBrand.updated_at = now;
                    _db.brands.Update(existingBrand);
                }
            }

            // 5) Payout account (upsert theo host_id + is_primary=true)
            if (!string.IsNullOrWhiteSpace(payload.AccountNumber) &&
                !string.IsNullOrWhiteSpace(payload.AccountName))
            {
                var payout = await _db.host_payout_accounts
                    .FirstOrDefaultAsync(p => p.host_id == hostProfile.host_id && p.is_primary == true, ct);

                if (payout == null)
                {
                    payout = new host_payout_account
                    {
                        account_id = Guid.NewGuid(),
                        host_id = hostProfile.host_id,
                        payment_method = payload.PaymentMethod,
                        account_name = payload.AccountName,
                        account_number = payload.AccountNumber,
                        bank_name = payload.BankName,
                        bank_branch = payload.BankBranch,
                        is_primary = true,
                        created_at = now
                    };
                    await _db.host_payout_accounts.AddAsync(payout, ct);
                }
                else
                {
                    payout.payment_method = payload.PaymentMethod;
                    payout.account_name = payload.AccountName;
                    payout.account_number = payload.AccountNumber;
                    payout.bank_name = payload.BankName;
                    payout.bank_branch = payload.BankBranch;
                    payout.is_primary = true;
                    _db.host_payout_accounts.Update(payout);
                }
            }

            payload.ReviewStatus = "approved";
            payload.ModeratorReviewedAt = now;
            payload.ModeratorId = moderatorId;

            draft.payload = HostRegistrationJson.SerializePayload(payload);
            draft.is_verified = true;
            draft.updated_at = now;
            _db.host_registration_drafts.Update(draft);

            await _db.SaveChangesAsync(ct);
            await trx.CommitAsync(ct);

            // Gửi email sau khi commit — lỗi gửi mail không rollback tài khoản đã tạo.
            try
            {
                var token = _jwt.CreateHostRegistrationToken(
                    hostUser.user_id,
                    hostUser.email,
                    HostRegistrationTokenUses.SetPassword,
                    TimeSpan.FromMinutes(30));

                var setPasswordLink = BuildLink(_config["HostRegistration:SetPasswordUrlTemplate"], token);
                var body =
                    $"<p>Tài khoản Host đã được duyệt. Vui lòng <a href=\"{setPasswordLink}\">bấm vào đây để đặt mật khẩu</a> (hiệu lực 30 phút).</p>";
                await _email.SendEmailAsync(new MailData
                {
                    EmailToId = hostUser.email,
                    EmailToName = hostUser.username,
                    EmailSubject = "BoxHub — Tài khoản Host đã được duyệt",
                    EmailBody = body
                });
            }
            catch
            {
                // Hangfire có thể retry job; nếu user đã tồn tại, bước đầu idempotent sẽ thoát.
            }
        }
        catch
        {
            await trx.RollbackAsync(ct);
            throw;
        }
    }

    private static string BuildLink(string? template, string token)
    {
        if (string.IsNullOrWhiteSpace(template))
            return token;
        return template.Contains("{token}", StringComparison.Ordinal)
            ? template.Replace("{token}", Uri.EscapeDataString(token), StringComparison.Ordinal)
            : $"{template.TrimEnd('/')}?token={Uri.EscapeDataString(token)}";
    }
}
