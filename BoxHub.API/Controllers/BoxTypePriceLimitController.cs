using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Enums.Pricings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers
{
    [ApiController]
    [Route("api/Admin/box-type-price-limits")]
    [Authorize(Roles = "ADMIN")] // bật khi có auth
    public class BoxTypePriceLimitController : ControllerBase
    {
        private readonly IBoxTypePriceLimitService _service;

        public BoxTypePriceLimitController(IBoxTypePriceLimitService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("by-type")]
        public async Task<IActionResult> GetByType(
            [FromQuery] CapacityType capacityType,
            [FromQuery] BoxClass? boxClass)
        {
            var result = await _service.GetByCapacityAndClassAsync(capacityType, boxClass);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBoxTypePriceLimitRequest req,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateBoxTypePriceLimitRequest req,
            CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:guid}/toggle-active")]
        public async Task<IActionResult> ToggleActive(
            Guid id,
            CancellationToken ct)
        {
            var result = await _service.ToggleActiveAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
