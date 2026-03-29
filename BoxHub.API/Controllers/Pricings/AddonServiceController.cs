using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers.Pricings
{
    [Route("api/admin/pricing/addon-services")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AddonServiceController : ControllerBase
    {
        private readonly IAddonServiceService _service;

        public AddonServiceController(IAddonServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? isActive,
            CancellationToken ct)
        {
            var result = await _service.GetAllAsync(isActive, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAddonServiceRequest req,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{serviceId:guid}")]
        public async Task<IActionResult> Update(
            Guid serviceId,
            [FromBody] UpdateAddonServiceRequest req,
            CancellationToken ct)
        {
            var result = await _service.UpdateAsync(serviceId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{serviceId:guid}/toggle")]
        public async Task<IActionResult> ToggleActive(
            Guid serviceId,
            CancellationToken ct)
        {
            var result = await _service.ToggleActiveAsync(serviceId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> Delete(
            Guid serviceId,
            CancellationToken ct)
        {
            var result = await _service.DeleteAsync(serviceId, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
