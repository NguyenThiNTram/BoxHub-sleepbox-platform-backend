namespace BoxHub.Application.DTOs.Responses.Otp;

public sealed class SendOtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}

