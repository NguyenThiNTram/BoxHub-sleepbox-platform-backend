using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Domain.Enums.Pricings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers.Pricings
{
    [Route("api/Admin/Platform-fee-config")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class PlatformFeeConfigController : ControllerBase
    {
        private readonly IPlatformFeeConfigService _service;

        public PlatformFeeConfigController(IPlatformFeeConfigService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken ct)
        {
            var result = await _service.GetAllActiveAsync(ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetTimeline(
            [FromQuery] FeeCode feeCode,
            [FromQuery] Guid? hostId,
            CancellationToken ct)
        {
            var result = await _service.GetTimelineAsync(feeCode, hostId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] CreatePlatformFeeConfigRequest req,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{configId:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(
            Guid configId,
            CancellationToken ct)
        {
            var result = await _service.DeactivateAsync(configId, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
