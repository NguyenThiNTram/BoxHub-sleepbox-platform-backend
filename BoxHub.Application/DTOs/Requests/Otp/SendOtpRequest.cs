using BoxHub.Domain.Enums;

namespace BoxHub.Application.DTOs.Requests.Otp;

public sealed class SendOtpRequest
{
    public string Email { get; set; } = "";
    public OTPPurpose Purpose { get; set; } = OTPPurpose.VERIFY_EMAIL;
}

