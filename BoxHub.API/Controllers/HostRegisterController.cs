using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

[ApiController]
[Route("api/host/register")]
public sealed class HostRegisterController : ControllerBase
{
    private readonly IHostRegistrationService _hostRegistration;

    public HostRegisterController(IHostRegistrationService hostRegistration)
    {
        _hostRegistration = hostRegistration;
    }

    [HttpPost("draft")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDraft([FromForm] RegisterHostDraftForm form, CancellationToken ct)
    {
        var token = ReadBearerToken();
        var result = await _hostRegistration.CreateDraftAsync(token, form, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Created(string.Empty, result.Value);
    }

    [HttpGet("draft/{draftId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDraftForEdit(
        [FromRoute] Guid draftId,
        CancellationToken ct)
    {
        var token = ReadBearerToken();
        var result = await _hostRegistration.GetDraftForEditAsync(draftId, token, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    [HttpPut("draft/{draftId:guid}")]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid draftId,
        [FromForm] RegisterHostDraftForm form,
        CancellationToken ct)
    {
        var token = ReadBearerToken();
        var result = await _hostRegistration.UpdateDraftAsync(draftId, token, form, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    private string? ReadBearerToken()
    {
        if (Request.Headers.TryGetValue("Authorization", out var auth)
            && !string.IsNullOrWhiteSpace(auth))
        {
            var value = auth.ToString().Trim();
            if (value.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                var rest = value.Substring("Bearer".Length).Trim();
                if (!string.IsNullOrWhiteSpace(rest))
                    return rest;
            }
            return value;
        }
        return null;
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
