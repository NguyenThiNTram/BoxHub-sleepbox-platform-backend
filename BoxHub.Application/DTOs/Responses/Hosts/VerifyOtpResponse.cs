using System;

namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class VerifyOtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public Guid? DraftId { get; set; }
    public string? Token { get; set; }
}
