using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoxHub.Infrastructure.Repositories;

public sealed class HostRegistrationRepository : IHostRegistrationRepository
{
    private readonly BoxHubDbContext _db;

    public HostRegistrationRepository(BoxHubDbContext db)
    {
        _db = db;
    }

    public async Task<host_registration_draft?> GetDraftByIdAsync(Guid draftId, bool track, CancellationToken ct)
    {
        var q = _db.host_registration_drafts.AsQueryable();
        if (!track)
            q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(d => d.draft_id == draftId, ct);
    }

    public Task AddDraftAsync(host_registration_draft draft, CancellationToken ct) =>
        _db.host_registration_drafts.AddAsync(draft, ct).AsTask();

    public Task AddOtpAsync(email_otp otp, CancellationToken ct) =>
        _db.email_otps.AddAsync(otp, ct).AsTask();

    public Task UpdateDraftAsync(host_registration_draft draft, CancellationToken ct)
    {
        _db.host_registration_drafts.Update(draft);
        return Task.CompletedTask;
    }

    public Task UpdateOtpAsync(email_otp otp, CancellationToken ct)
    {
        _db.email_otps.Update(otp);
        return Task.CompletedTask;
    }

    public async Task<(List<host_registration_draft> Items, int TotalCount)> GetDraftsPagedAsync(
        int page,
        int pageSize,
        string? reviewStatusFilter,
        CancellationToken ct)
    {
        var q = _db.host_registration_drafts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(reviewStatusFilter))
        {
            var s = reviewStatusFilter.Trim().ToLowerInvariant();
            q = q.Where(d => d.payload.Contains($"\"reviewStatus\":\"{s}\""));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(d => d.created_at)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public Task<int> CountOtpsCreatedSinceAsync(string email, OTPPurpose purpose, DateTime sinceUtc, CancellationToken ct) =>
        _db.email_otps.CountAsync(
            o => o.email == email && o.purpose == purpose && o.created_at >= sinceUtc,
            ct);

    public async Task<email_otp?> GetLatestOtpForEmailAsync(string email, OTPPurpose purpose, CancellationToken ct) =>
        await _db.email_otps
            .Where(o => o.email == email && o.purpose == purpose)
            .OrderByDescending(o => o.created_at)
            .FirstOrDefaultAsync(ct);

    public async Task<email_otp?> FindValidOtpAsync(
        string email,
        string otpCode,
        OTPPurpose purpose,
        DateTime utcNow,
        CancellationToken ct) =>
        await _db.email_otps
            .AsNoTracking()
            .FirstOrDefaultAsync(
                o => o.email == email
                     && o.otp_code == otpCode
                     && o.purpose == purpose
                     && !o.is_used
                     && o.expire_at > utcNow,
                ct);

    public async Task<host_registration_draft?> FindLatestDraftByEmailAsync(string email, CancellationToken ct) =>
        await _db.host_registration_drafts
            .Where(d => d.email == email)
            .OrderByDescending(d => d.created_at)
            .FirstOrDefaultAsync(ct);

    public async Task<email_otp?> GetOtpByIdAsync(Guid otpId, bool track, CancellationToken ct)
    {
        var q = _db.email_otps.AsQueryable();
        if (!track)
            q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(o => o.otp_id == otpId, ct);
    }

    public async Task<host_registration_draft?> GetDraftByOtpIdAsync(Guid otpId, bool track, CancellationToken ct)
    {
        var q = _db.host_registration_drafts.AsQueryable();
        if (!track)
            q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(d => d.otp_id == otpId, ct);
    }

    public Task<bool> RepresentativeIdNumberTakenAsync(string normalizedRepresentativeId, CancellationToken ct)
    {
        var rep = normalizedRepresentativeId.Trim();
        return _db.host_profiles.AnyAsync(
            h => h.representative_id_number != null
                 && h.representative_id_number.ToUpper() == rep,
            ct);
    }

    public Task<bool> TaxCodeTakenAsync(string normalizedTaxCode, CancellationToken ct)
    {
        var t = normalizedTaxCode.Trim();
        return _db.host_profiles.AnyAsync(
            h => h.tax_code != null && h.tax_code.ToUpper() == t,
            ct);
    }

    public Task<bool> BrandNameTakenAsync(string normalizedBrandName, CancellationToken ct)
    {
        var n = normalizedBrandName.Trim();
        return _db.brands.AnyAsync(
            b => !b.is_deleted && b.brand_name.ToUpper() == n,
            ct);
    }
}
