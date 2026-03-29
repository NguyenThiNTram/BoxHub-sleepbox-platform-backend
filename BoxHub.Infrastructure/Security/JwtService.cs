using BoxHub.Application.Constants;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoxHub.Infrastructure.Auth;

public sealed class JwtService : IJwtService
{
    private const string TokenUseClaimType = "token_use";
    private const string DraftIdClaimType = "draft_id";

    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public AuthResponse GenerateAccessToken(user user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.role.ToString().ToUpperInvariant())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds
        );

        return new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = token.ValidTo,
            UserId = user.user_id,
            Email = user.email,
            Role = user.role.ToString().ToUpperInvariant()
        };
    }

    /// <inheritdoc />
    public string CreateHostRegistrationToken(Guid subjectId, string email, string tokenUse, TimeSpan lifetime)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["HostRegistrationAudience"] ?? "BoxHub.HostRegistration";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, subjectId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(TokenUseClaimType, tokenUse)
        };

        // Yêu cầu nghiệp vụ: token draft chứa draft_id (rõ ràng cho client parse).
        if (string.Equals(tokenUse, HostRegistrationTokenUses.DraftEdit, StringComparison.OrdinalIgnoreCase))
            claims.Add(new Claim(DraftIdClaimType, subjectId.ToString()));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public HostRegistrationTokenPayload? TryValidateHostRegistrationToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var jwtSection = _config.GetSection("Jwt");
        var audience = jwtSection["HostRegistrationAudience"] ?? "BoxHub.HostRegistration";

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, parameters, out var _);

            // JwtSecurityTokenHandler mặc định map inbound claims (sub/email -> ClaimTypes.*),
            // nên cần fallback cả 2 dạng để tránh null.
            var sub =
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            var email =
                principal.FindFirst(ClaimTypes.Email)?.Value
                ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            var tokenUse = principal.FindFirst(TokenUseClaimType)?.Value;
            if (sub == null || email == null || tokenUse == null || !Guid.TryParse(sub, out var subjectId))
                return null;
            return new HostRegistrationTokenPayload(subjectId, email, tokenUse);
        }
        catch (Exception ex)
        {
            try
            {
                var first50 = string.IsNullOrEmpty(token)
                    ? string.Empty
                    : token.Substring(0, Math.Min(50, token.Length));

                Console.WriteLine($"[HostRegistrationToken] Invalid token. token length={token?.Length}");
                Console.WriteLine($"[HostRegistrationToken] raw token (first 50)={first50}");
                Console.WriteLine($"[HostRegistrationToken] exception={ex.GetType().FullName}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[HostRegistrationToken] inner={ex.InnerException.GetType().FullName}: {ex.InnerException.Message}");
            }
            catch
            {
                // ignore logging errors
            }

            return null;
        }
    }
}
