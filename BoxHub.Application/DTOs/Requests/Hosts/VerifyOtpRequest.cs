namespace BoxHub.Application.DTOs.Requests.Hosts;

public sealed class VerifyOtpRequest
{
    public string Email { get; set; } = "";
    public string OtpCode { get; set; } = "";
}
