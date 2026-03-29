using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BoxHub.Application.DTOs.Requests.Hosts;

/// <summary>multipart/form-data cho đăng ký / cập nhật draft Host (tên field snake_case theo API).</summary>
public sealed class RegisterHostDraftForm
{
    [FromForm(Name = "email")]
    public string Email { get; set; } = "";

    [FromForm(Name = "username")]
    public string? Username { get; set; }

    [FromForm(Name = "phone")]
    public string? Phone { get; set; }

    [FromForm(Name = "first_name")]
    public string? FirstName { get; set; }

    [FromForm(Name = "last_name")]
    public string? LastName { get; set; }

    // --- Host profile  ---
    [FromForm(Name = "representative_id_name")]
    public string? RepresentativeIdName { get; set; }

    [FromForm(Name = "representative_id_number")]
    public string? RepresentativeIdNumber { get; set; }

    [FromForm(Name = "representative_front_url")]
    public IFormFile? RepresentativeFrontUrl { get; set; }

    [FromForm(Name = "representative_back_url")]
    public IFormFile? RepresentativeBackUrl { get; set; }

    [FromForm(Name = "tax_code")]
    public string? TaxCode { get; set; }

    // --- Brand & business info ---
    [FromForm(Name = "brand_name")]
    public string? BrandName { get; set; }

    [FromForm(Name = "brand_avatar")]
    public IFormFile? BrandAvatar { get; set; }

    [FromForm(Name = "business_name")]
    public string? BusinessName { get; set; }

    // --- Address ---
    [FromForm(Name = "address_district")]
    public string? AddressDistrict { get; set; }

    [FromForm(Name = "address_ward")]
    public string? AddressWard { get; set; }

    [FromForm(Name = "address_detail")]
    public string? AddressDetail { get; set; }

    [FromForm(Name = "company_registration")]
    public IFormFile? CompanyRegistrationFile { get; set; }

    // --- Payout / bank account ---
    [FromForm(Name = "bank_name")]
    public string? BankName { get; set; }

    [FromForm(Name = "account_number")]
    public string? AccountNumber { get; set; }

    [FromForm(Name = "account_name")]
    public string? AccountName { get; set; }

    [FromForm(Name = "payment_method")]
    public string? PaymentMethod { get; set; }
}
