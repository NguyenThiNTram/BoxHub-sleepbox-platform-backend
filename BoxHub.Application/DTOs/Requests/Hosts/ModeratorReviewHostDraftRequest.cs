namespace BoxHub.Application.DTOs.Requests.Hosts;

public sealed class ModeratorReviewHostDraftRequest
{
    /// <summary>approve | reject</summary>
    public string Action { get; set; } = "";

    public string? RejectReason { get; set; }
    public List<ModeratorDocumentReviewItem>? DocumentReviews { get; set; }
}

public sealed class ModeratorDocumentReviewItem
{
    public string DocumentType { get; set; } = "";
    public string Status { get; set; } = "";
    public string? RejectReason { get; set; }
}
