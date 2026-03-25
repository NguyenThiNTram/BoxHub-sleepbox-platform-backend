using BoxHub.Application.Models.HostRegistration;

namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class ModeratorHostDraftDetailResponse
{
    public Guid DraftId { get; set; }
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public bool IsVerified { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public HostDraftPayloadModel Payload { get; set; } = new();
}
