using System.Text.Json;
using BoxHub.Application.Helpers;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Models.HostRegistration;
using BoxHub.Application.Constants;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Data;
using BoxHub.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BoxHub.Infrastructure.Services;

/// <summary>
/// Tạo User Host + profile + tài liệu sau khi moderator approve (transaction DB).
/// </summary>
public sealed class CreateHostAccountWorker : ICreateHostAccountWorker
{
    private readonly BoxHubDbContext _db;
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public CreateHostAccountWorker(
        BoxHubDbContext db,
        IUserRepository users,
        IJwtService jwt,
        IEmailService email,
        IConfiguration config)
    {
        _db = db;
        _users = users;
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
            var existing = await _users.GetByEmailAsync(emailNorm, ct);
            if (existing != null)
            {
                await trx.RollbackAsync(ct);
                return;
            }

            var now = DateTime.UtcNow;

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
                password_hash = null,
                created_at = now
            };

            await _users.AddAsync(newUser, ct);

            DateOnly? dob = null;
            if (!string.IsNullOrWhiteSpace(payload.DateOfBirth) &&
                DateOnly.TryParse(payload.DateOfBirth, out var parsedDob))
                dob = parsedDob;

            var profile = new user_profile
            {
                profile_id = Guid.NewGuid(),
                user_id = newUser.user_id,
                first_name = payload.FirstName,
                last_name = payload.LastName,
                gender = payload.Gender,
                date_of_birth = dob,
                updated_at = now
            };
            await _users.CreateProfileAsync(profile, ct);

            var hostProfile = new host_profile
            {
                host_id = Guid.NewGuid(),
                user_id = newUser.user_id,
                representative_name = payload.RepresentativeName,
                representative_id_number = payload.RepresentativeIdNumber,
                tax_code = payload.TaxCode,
                business_address = payload.BusinessAddress,
                verified_status = "APPROVED",
                verified_at = now,
                verified_by = moderatorId,
                created_at = now,
                updated_at = now
            };
            await _db.host_profiles.AddAsync(hostProfile, ct);

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
                    newUser.user_id,
                    newUser.email,
                    HostRegistrationTokenUses.SetPassword,
                    TimeSpan.FromMinutes(30));

                var setPasswordLink = BuildLink(_config["HostRegistration:SetPasswordUrlTemplate"], token);
                var body =
                    $"<p>Tài khoản Host đã được duyệt. Vui lòng <a href=\"{setPasswordLink}\">bấm vào đây để đặt mật khẩu</a> (hiệu lực 30 phút).</p>";
                await _email.SendEmailAsync(new MailData
                {
                    EmailToId = newUser.email,
                    EmailToName = newUser.username,
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
