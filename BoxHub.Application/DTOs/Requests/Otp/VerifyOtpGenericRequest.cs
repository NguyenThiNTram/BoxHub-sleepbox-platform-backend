using BoxHub.Domain.Enums;

namespace BoxHub.Application.DTOs.Requests.Otp;

public sealed class VerifyOtpGenericRequest
{
    public string Email { get; set; } = "";
    public string OtpCode { get; set; } = "";
    public OTPPurpose Purpose { get; set; } = OTPPurpose.VERIFY_EMAIL;
}

