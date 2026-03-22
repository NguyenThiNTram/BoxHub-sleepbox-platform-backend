using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BoxHub.API.Controllers;

/// <summary>Đăng ký Host — tạo / cập nhật bản nháp (multipart).</summary>
[ApiController]
[Route("api/host/register")]
public sealed class HostRegisterController : ControllerBase
{
    private readonly IHostRegistrationService _hostRegistration;

    public HostRegisterController(IHostRegistrationService hostRegistration)
    {
        _hostRegistration = hostRegistration;
    }

    /// <summary>POST /api/host/register/draft — tạo draft + gửi OTP.</summary>
    [HttpPost("draft")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDraft([FromForm] RegisterHostDraftForm form, CancellationToken ct)
    {
        var result = await _hostRegistration.RegisterDraftAsync(form, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Created(string.Empty, result.Value);
    }

    /// <summary>GET /api/host/register/draft/{draftId} — lấy dữ liệu đã lưu để fill form (token query hoặc header).</summary>
    [HttpGet("draft/{draftId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDraftForEdit([FromRoute] Guid draftId, CancellationToken ct)
    {
        var token = ReadTokenFromQueryOrHeader();
        var result = await _hostRegistration.GetDraftForEditAsync(draftId, token, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    /// <summary>PUT /api/host/register/draft/{draftId} — token query hoặc header.</summary>
    [HttpPut("draft/{draftId:guid}")]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid draftId,
        [FromForm] RegisterHostDraftForm form,
        CancellationToken ct)
    {
        var token = ReadTokenFromQueryOrHeader();
        var result = await _hostRegistration.UpdateDraftAsync(draftId, token, form, ct);
        if (!result.IsSuccess)
            return ProblemResult(result.ErrorCode, result.ErrorMessage, result.HttpStatus ?? 400);

        return Ok(result.Value);
    }

    private string? ReadTokenFromQueryOrHeader()
    {
        if (Request.Headers.TryGetValue("token", out var h) && !string.IsNullOrWhiteSpace(h))
            return h.ToString();
        return Request.Query["token"].FirstOrDefault();
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
