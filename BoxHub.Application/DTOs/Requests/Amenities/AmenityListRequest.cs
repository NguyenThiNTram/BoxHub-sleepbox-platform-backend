using Microsoft.AspNetCore.Mvc;

namespace BoxHub.Application.DTOs.Requests.Amenities;

public sealed class AmenityListRequest
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    [FromQuery(Name = "type")]
    public string? Type { get; set; }

    [FromQuery(Name = "pageNumber")]
    public int PageNumber { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 50;
}

