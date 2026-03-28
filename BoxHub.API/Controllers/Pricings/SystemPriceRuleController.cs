using BoxHub.Application.DTOs.Requests;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Domain.Enums.Pricings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers.Pricings
{
    [ApiController]
    [Route("api/Admin/system-price-rules")]
    [Authorize(Roles = "ADMIN")]
    public class SystemPriceRuleController : ControllerBase
    {
        private readonly ISystemPriceRuleService _service;

        public SystemPriceRuleController(ISystemPriceRuleService service)
        {
            _service = service;
        }

        // rule price

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PricingMode? mode,
            [FromQuery] bool? isActive,
            [FromQuery] PagingRequest paging,
            CancellationToken ct)
        {
            var result = await _service.GetAllAsync(mode, isActive, paging, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{ruleId:guid}")]
        public async Task<IActionResult> GetDetail(Guid ruleId, CancellationToken ct)
        {
            var result = await _service.GetDetailAsync(ruleId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePriceRuleRequest req,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{ruleId:guid}")]
        public async Task<IActionResult> Update(
            Guid ruleId,
            [FromBody] UpdatePriceRuleRequest req,
            CancellationToken ct)
        {
            var result = await _service.UpdateAsync(ruleId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{ruleId:guid}/toggle-active")]
        public async Task<IActionResult> ToggleActive(Guid ruleId, CancellationToken ct)
        {
            var result = await _service.ToggleActiveAsync(ruleId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{ruleId:guid}")]
        public async Task<IActionResult> Delete(Guid ruleId, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(ruleId, ct);
            return StatusCode(result.StatusCode, result);
        }

        // factor

        [HttpGet("{ruleId:guid}/factors")]
        public async Task<IActionResult> GetFactors(Guid ruleId, CancellationToken ct)
        {
            var result = await _service.GetFactorsAsync(ruleId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{ruleId:guid}/factors")]
        public async Task<IActionResult> AddFactor(
            Guid ruleId,
            [FromBody] PricingFactor req,
            CancellationToken ct)
        {
            var result = await _service.AddFactorAsync(ruleId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("factors/{factorId:guid}")]
        public async Task<IActionResult> UpdateFactor(
            Guid factorId,
            [FromBody] PricingFactor req,
            CancellationToken ct)
        {
            var result = await _service.UpdateFactorAsync(factorId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("factors/{factorId:guid}")]
        public async Task<IActionResult> DeleteFactor(Guid factorId, CancellationToken ct)
        {
            var result = await _service.DeleteFactorAsync(factorId, ct);
            return StatusCode(result.StatusCode, result);
        }

        // combo

        [HttpGet("{ruleId:guid}/combos")]
        public async Task<IActionResult> GetCombos(Guid ruleId, CancellationToken ct)
        {
            var result = await _service.GetCombosAsync(ruleId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{ruleId:guid}/combos")]
        public async Task<IActionResult> AddCombo(
            Guid ruleId,
            [FromBody] PricingCombo req,
            CancellationToken ct)
        {
            var result = await _service.AddComboAsync(ruleId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("combos/{comboId:guid}")]
        public async Task<IActionResult> UpdateCombo(
            Guid comboId,
            [FromBody] PricingCombo req,
            CancellationToken ct)
        {
            var result = await _service.UpdateComboAsync(comboId, req, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("combos/{comboId:guid}")]
        public async Task<IActionResult> DeleteCombo(Guid comboId, CancellationToken ct)
        {
            var result = await _service.DeleteComboAsync(comboId, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
