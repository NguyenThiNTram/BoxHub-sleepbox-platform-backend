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
    public string? Gender { get; set; }
    /// <summary>yyyy-MM-dd nếu có.</summary>
    public string? DateOfBirth { get; set; }
    public string? RepresentativeName { get; set; }
    public string? RepresentativeIdNumber { get; set; }
    public string? TaxCode { get; set; }
    public string? BusinessAddress { get; set; }
    public List<HostDraftDocumentItemResponse> Documents { get; set; } = new();
    public string ReviewStatus { get; set; } = "";
    public DateTime? EmailVerifiedAt { get; set; }
}

public sealed class HostDraftDocumentItemResponse
{
    public string DocumentType { get; set; } = "";
    public List<string> Attachments { get; set; } = new();
}
