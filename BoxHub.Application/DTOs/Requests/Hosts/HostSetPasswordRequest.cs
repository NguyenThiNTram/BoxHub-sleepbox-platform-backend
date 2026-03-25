namespace BoxHub.Application.DTOs.Requests.Hosts;

public sealed class HostSetPasswordRequest
{
    public string NewPassword { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}
