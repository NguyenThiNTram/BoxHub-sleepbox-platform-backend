using System;

namespace BoxHub.Application.Auth;

public sealed class RegisterGuestRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public sealed class LoginGuestRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

