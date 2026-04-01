using BoxHub.Application.DTOs.Requests.Otp;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.DTOs.Responses.Otp;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Interfaces.Services;

public interface IOtpService
{
    Task<Result<SendOtpResponse>> SendAsync(SendOtpRequest request, CancellationToken ct);

    Task<Result<VerifyOtpResponse>> VerifyAsync(VerifyOtpGenericRequest request, CancellationToken ct);

    Task<Result<SimpleMessageResponse>> ResendAsync(ResendOtpGenericRequest request, CancellationToken ct);
}

