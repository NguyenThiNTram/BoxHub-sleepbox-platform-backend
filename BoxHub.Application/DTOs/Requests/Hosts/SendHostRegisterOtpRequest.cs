using System.ComponentModel.DataAnnotations;

namespace BoxHub.Application.DTOs.Requests.Hosts;

/// <summary>Request JSON để gửi OTP đăng ký Host (bước 1: nhập email).</summary>
public sealed class SendHostRegisterOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}

