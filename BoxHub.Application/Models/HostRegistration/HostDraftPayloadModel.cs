namespace BoxHub.Application.Models.HostRegistration;

/// <summary>
/// Cấu trúc JSON lưu trong host_registration_drafts.payload (jsonb) — camelCase khi serialize.
/// </summary>
public sealed class HostDraftPayloadModel
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    /// <summary>Chuỗi ngày yyyy-MM-dd.</summary>
    public string? DateOfBirth { get; set; }
    public string? RepresentativeName { get; set; }
    public string? RepresentativeIdNumber { get; set; }
    public string? TaxCode { get; set; }
    public string? BusinessAddress { get; set; }

    public List<HostDraftDocumentModel> Documents { get; set; } = new();

    /// <summary>pending | approved | rejected</summary>
    public string ReviewStatus { get; set; } = "pending";

    public string? RejectReason { get; set; }
    public List<HostDraftDocumentReviewModel>? DocumentReviews { get; set; }
    public DateTime? ModeratorReviewedAt { get; set; }
    public Guid? ModeratorId { get; set; }

    /// <summary>Thời điểm OTP email được xác nhật (UTC).</summary>
    public DateTime? EmailVerifiedAt { get; set; }
}

public sealed class HostDraftDocumentModel
{
    public string DocumentType { get; set; } = "";
    public List<string> Attachments { get; set; } = new();
}

public sealed class HostDraftDocumentReviewModel
{
    public string DocumentType { get; set; } = "";
    public string Status { get; set; } = "";
    public string? RejectReason { get; set; }
}
