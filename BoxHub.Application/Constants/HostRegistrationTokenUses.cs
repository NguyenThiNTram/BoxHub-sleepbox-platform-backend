namespace BoxHub.Application.Constants;

/// <summary>Giá trị claim token_use cho JWT luồng đăng ký Host.</summary>
public static class HostRegistrationTokenUses
{
    /// <summary>Token sau khi verify OTP email, dùng để tạo/cập nhật draft theo email (không gắn draftId).</summary>
    public const string EmailVerified = "email_verified";
    public const string DraftEdit = "draft_edit";
    public const string SetPassword = "set_password";
}
