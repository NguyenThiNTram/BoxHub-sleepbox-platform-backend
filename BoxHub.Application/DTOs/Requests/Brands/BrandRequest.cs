using Microsoft.AspNetCore.Http;

namespace BoxHub.Application.DTOs.Requests.Brands;

public sealed class BrandRequest
{
    public string? BrandName { get; set; }

    public IFormFile? BrandAvatar { get; set; }
}
