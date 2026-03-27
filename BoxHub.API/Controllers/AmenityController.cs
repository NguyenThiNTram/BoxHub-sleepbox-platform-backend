using BoxHub.Application.DTOs.Requests.Amenities;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/amenities")]
[Authorize(Roles = "HOST")]
public sealed class AmenityController : ControllerBase
{
    private readonly IAmenityService _svc;

    public AmenityController(IAmenityService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] AmenityListRequest request, CancellationToken ct)
    {
        var result = await _svc.ListAsync(request, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var result = await _svc.GetByIdAsync(id, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAmenityRequest request, CancellationToken ct)
    {
        var result = await _svc.CreateAsync(request, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAmenityRequest request, CancellationToken ct)
    {
        var result = await _svc.UpdateAsync(id, request, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var result = await _svc.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return NoContent();
    }

    private static ObjectResult ProblemResult(string? code, string? message, int status) =>
        new(new ProblemDetails
        {
            Title = code ?? ErrorCodes.ValidationFailed,
            Detail = message,
            Status = status
        })
        {
            StatusCode = status
        };
}

