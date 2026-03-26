namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class ModeratorHostDraftListResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<ModeratorHostDraftSummaryResponse> Items { get; set; } = new();
}

public sealed class ModeratorHostDraftSummaryResponse
{
    public Guid DraftId { get; set; }
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string ReviewStatus { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
}
