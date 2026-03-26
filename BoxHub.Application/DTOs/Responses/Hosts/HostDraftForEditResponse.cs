namespace BoxHub.Application.DTOs.Responses.Hosts;

/// <summary>Dữ liệu bản nháp để form chỉnh sửa (GET trước khi PUT).</summary>
public sealed class HostDraftForEditResponse
{
    public Guid DraftId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // host_profile
    public string? RepresentativeIdName { get; set; }
    public string? RepresentativeIdNumber { get; set; }
    public string? RepresentativeFrontUrl { get; set; }
    public string? RepresentativeBackUrl { get; set; }
    public string? TaxCode { get; set; }

    // brand & business
    public string? BrandName { get; set; }
    public string? BrandAvatarUrl { get; set; }
    public string? BusinessName { get; set; }

    // Address
    public string? AddressDistrict { get; set; }
    public string? AddressWard { get; set; }
    public string? AddressDetail { get; set; }

    // payout
    public string? PaymentMethod { get; set; }
    public string? BankName { get; set; }
    public string? BankBranch { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }

    public List<HostDraftDocumentItemResponse> Documents { get; set; } = new();
    public string ReviewStatus { get; set; } = "";
    public DateTime? EmailVerifiedAt { get; set; }
}

public sealed class HostDraftDocumentItemResponse
{
    public string DocumentType { get; set; } = "";
    public List<string> Attachments { get; set; } = new();
}
