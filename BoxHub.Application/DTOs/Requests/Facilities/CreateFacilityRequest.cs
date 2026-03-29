using Microsoft.AspNetCore.Http;

namespace BoxHub.Application.DTOs.Requests.Facilities;

/// <summary>Tạo cơ sở (multipart): kèm bắt buộc 2 file giấy tờ (raw/PDF).</summary>
public sealed class CreateFacilityRequest
{
    public string? FacilityName { get; set; }

    public string? Description { get; set; }

    public string? AddressStreet { get; set; }
    public string? AddressWard { get; set; }
    public string? AddressDistrict { get; set; }
    public string? AddressCity { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string? HouseRules { get; set; }

    public int? CleaningBufferMinutes { get; set; }

    /// <summary>Giấy chứng nhận đăng ký cơ sở kinh doanh (raw/PDF).</summary>
    public IFormFile? BusinessLicense { get; set; }

    /// <summary>Giấy chứng nhận / biên bản PCCC (raw/PDF).</summary>
    public IFormFile? PcccDocument { get; set; }
}
