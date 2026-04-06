using System.ComponentModel.DataAnnotations;

namespace BoxHub.Application.DTOs.Requests.Auths;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
