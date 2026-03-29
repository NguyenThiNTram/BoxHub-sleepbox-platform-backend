namespace BoxHub.Application.DTOs.Responses.Amenities;

public sealed class AmenityResponse
{
    public int AmenityId { get; set; }
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string? Description { get; set; }
}

