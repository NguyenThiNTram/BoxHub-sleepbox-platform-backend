namespace BoxHub.Application.DTOs.Responses.Facilities;

public sealed class CreateFacilityResponse
{
    public Guid FacilityId { get; set; }
    public Guid BrandId { get; set; }
    public string FacilityName { get; set; } = "";
    public string FacilityStatus { get; set; } = "";
}
