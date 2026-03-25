using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers
{
    [ApiController]
    [Route("api/Facility")]
    public class FacilitiesController : ControllerBase
    {
        private readonly IFacilityService _facilityService;

        public FacilitiesController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] FacilitySearchRequest request,
            CancellationToken ct)
        {
            var result = await _facilityService.SearchAsync(request, ct);

            return Ok(result);
        }
    }
}
