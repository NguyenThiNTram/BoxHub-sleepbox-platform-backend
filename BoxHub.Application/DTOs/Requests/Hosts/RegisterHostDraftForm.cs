using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.Application.DTOs.Requests.Hosts;

/// <summary>multipart/form-data cho đăng ký / cập nhật draft Host (tên field snake_case theo API).</summary>
public sealed class RegisterHostDraftForm
{
    [FromForm(Name = "username")]
    public string Username { get; set; } = "";

    [FromForm(Name = "email")]
    public string Email { get; set; } = "";

    [FromForm(Name = "phone")]
    public string? Phone { get; set; }

    [FromForm(Name = "first_name")]
    public string? FirstName { get; set; }

    [FromForm(Name = "last_name")]
    public string? LastName { get; set; }

    [FromForm(Name = "gender")]
    public string? Gender { get; set; }

    [FromForm(Name = "date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [FromForm(Name = "representative_name")]
    public string? RepresentativeName { get; set; }

    [FromForm(Name = "representative_id_number")]
    public string? RepresentativeIdNumber { get; set; }

    [FromForm(Name = "tax_code")]
    public string? TaxCode { get; set; }

    [FromForm(Name = "business_address")]
    public string? BusinessAddress { get; set; }

    /// <summary>Giấy phép kinh doanh — document_type: BUSINESS_LICENSE</summary>
    [FromForm(Name = "business_license")]
    public IFormFile? BusinessLicense { get; set; }

    /// <summary>Giấy chứng nhận thuế — TAX_CERTIFICATE</summary>
    [FromForm(Name = "tax_certificate")]
    public IFormFile? TaxCertificate { get; set; }

    /// <summary>CCCD / CMND — IDENTITY_CARD</summary>
    [FromForm(Name = "identity_card")]
    public IFormFile? IdentityCard { get; set; }

    /// <summary>Đăng ký doanh nghiệp — COMPANY_REGISTRATION</summary>
    [FromForm(Name = "company_registration")]
    public IFormFile? CompanyRegistration { get; set; }

    /// <summary>PCCC — PCCC</summary>
    [FromForm(Name = "pccc")]
    public IFormFile? Pccc { get; set; }
}
