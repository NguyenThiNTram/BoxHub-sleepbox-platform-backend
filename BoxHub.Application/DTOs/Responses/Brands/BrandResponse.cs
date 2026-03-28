using BoxHub.Domain.Enums;

namespace BoxHub.Application.DTOs.Responses.Brands;

public sealed class BrandResponse
{
    public Guid BrandId { get; set; }
    public Guid HostId { get; set; }
    public string BrandName { get; set; } = "";
    public string? BrandAvatar { get; set; }
    public BrandStatus Status { get; set; }
}
