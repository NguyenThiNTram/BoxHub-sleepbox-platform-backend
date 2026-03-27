namespace BoxHub.Application.DTOs.Requests.Amenities;

public sealed class UpdateAmenityRequest
{
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string? Description { get; set; }
}

