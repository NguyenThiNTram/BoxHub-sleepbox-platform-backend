using BoxHub.Application.DTOs.Responses;
using BoxHub.Domain.Entities;

namespace BoxHub.Application.Interfaces.Services;

public interface IJwtService
{
    AuthResponse GenerateAccessToken(user user);

    /// <summary>JWT HS256 cho luồng Host (draft / đặt mật khẩu), audience riêng.</summary>
    string CreateHostRegistrationToken(Guid subjectId, string email, string tokenUse, TimeSpan lifetime);

    /// <summary>Trả về null nếu token không hợp lệ hoặc audience sai.</summary>
    HostRegistrationTokenPayload? TryValidateHostRegistrationToken(string token);
}

/// <param name="SubjectId">draft_id (draft_edit) hoặc user_id (set_password).</param>
/// <param name="TokenUse">draft_edit | set_password</param>
public sealed record HostRegistrationTokenPayload(Guid SubjectId, string Email, string TokenUse);