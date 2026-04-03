using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;

namespace BoxHub.Application.Services;

public sealed class HostPayoutService : IHostPayoutService
{
    private readonly IHostPayoutRepository _payouts;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public HostPayoutService(IHostPayoutRepository payouts, IUserRepository users, IUnitOfWork uow)
    {
        _payouts = payouts;
        _users = users;
        _uow = uow;
    }

    public async Task<HostPayoutAccountResponse> GetPayoutForCurrentHostAsync(Guid userId, CancellationToken ct)
    {
        await EnsureHostAsync(userId, ct);
        var host = await _payouts.GetHostProfileWithPayoutByUserIdAsync(userId, track: false, ct);
        if (host == null)
            throw new ApiException(ErrorCodes.HostProfileNotFound, "Không tìm thấy hồ sơ Host.", 404);

        var p = host.host_payout_account;
        if (p == null)
        {
            return new HostPayoutAccountResponse
            {
                AccountId = null,
                HostId = host.host_id,
                PaymentMethod = null,
                AccountName = null,
                AccountNumber = null,
                BankName = null,
                CreatedAt = null,
                UpdatedAt = null
            };
        }

        return Map(p, host.host_id);
    }

    public async Task<HostPayoutAccountResponse> UpdatePayoutForCurrentHostAsync(
        Guid userId,
        UpdateHostPayoutAccountRequest request,
        CancellationToken ct)
    {
        await EnsureHostAsync(userId, ct);
        var host = await _payouts.GetHostProfileWithPayoutByUserIdAsync(userId, track: true, ct);
        if (host == null)
            throw new ApiException(ErrorCodes.HostProfileNotFound, "Không tìm thấy hồ sơ Host.", 404);

        var now = DateTime.UtcNow;
        if (host.host_payout_account == null)
        {
            var created = new host_payout_account
            {
                account_id = Guid.NewGuid(),
                host_id = host.host_id,
                payment_method = request.PaymentMethod.Trim(),
                account_name = string.IsNullOrWhiteSpace(request.AccountName) ? null : request.AccountName.Trim(),
                account_number = request.AccountNumber.Trim(),
                bank_name = string.IsNullOrWhiteSpace(request.BankName) ? null : request.BankName.Trim(),
                is_primary = true,
                created_at = now,
                updated_at = now
            };
            await _payouts.AddPayoutAsync(created, ct);
        }
        else
        {
            var p = host.host_payout_account;
            p.payment_method = request.PaymentMethod.Trim();
            p.account_name = string.IsNullOrWhiteSpace(request.AccountName) ? null : request.AccountName.Trim();
            p.account_number = request.AccountNumber.Trim();
            p.bank_name = string.IsNullOrWhiteSpace(request.BankName) ? null : request.BankName.Trim();
            p.is_primary = true;
            p.updated_at = now;
        }

        await _uow.SaveChangesAsync(ct);

        var refreshed = await _payouts.GetHostProfileWithPayoutByUserIdAsync(userId, track: false, ct);
        var payout = refreshed!.host_payout_account!;
        return Map(payout, host.host_id);
    }

    private async Task EnsureHostAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null)
            throw new ApiException(ErrorCodes.UserNotFound, "Không tìm thấy người dùng.", 404);
        if (user.role != UserRole.Host)
            throw new ApiException(ErrorCodes.Forbidden, "Chỉ tài khoản Host mới được thao tác.", 403);
    }

    private static HostPayoutAccountResponse Map(host_payout_account p, Guid hostId) =>
        new()
        {
            AccountId = p.account_id,
            HostId = hostId,
            PaymentMethod = p.payment_method,
            AccountName = p.account_name,
            AccountNumber = p.account_number,
            BankName = p.bank_name,
            CreatedAt = p.created_at,
            UpdatedAt = p.updated_at
        };
}
