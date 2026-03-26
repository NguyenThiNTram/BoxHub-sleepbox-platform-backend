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

    // --- Host profile ---
    public string? RepresentativeIdName { get; set; }
    public string? RepresentativeIdNumber { get; set; }
    public string? RepresentativeFrontUrl { get; set; }
    public string? RepresentativeBackUrl { get; set; }
    public string? TaxCode { get; set; }
    public string? BusinessName { get; set; }

    // --- Address ---
    public string? AddressDistrict { get; set; }
    public string? AddressWard { get; set; }
    public string? AddressDetail { get; set; }

    // --- Brand ---
    public string? BrandName { get; set; }
    public string? BrandAvatarUrl { get; set; }

    // --- Payout account ---
    /// <summary>Giá trị enum string (ví dụ: BANK_TRANSFER / PAY_AT_BASE).</summary>
    public string? PaymentMethod { get; set; }
    public string? BankName { get; set; }
    public string? BankBranch { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }

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
