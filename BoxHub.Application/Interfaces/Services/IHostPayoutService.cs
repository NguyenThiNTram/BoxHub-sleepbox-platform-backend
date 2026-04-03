using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;

namespace BoxHub.Application.Interfaces.Services;

public interface IHostPayoutService
{
    Task<HostPayoutAccountResponse> GetPayoutForCurrentHostAsync(Guid userId, CancellationToken ct);

    Task<HostPayoutAccountResponse> UpdatePayoutForCurrentHostAsync(Guid userId, UpdateHostPayoutAccountRequest request, CancellationToken ct);
}
