using BoxHub.Application.DTOs.Requests.Hosts;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Shared.Results;

namespace BoxHub.Application.Interfaces.Services;

public interface IHostRegistrationService
{
    Task<Result<RegisterHostDraftResponse>> CreateDraftAsync(string? token, RegisterHostDraftForm form, CancellationToken ct);

    Task<Result<SimpleMessageResponse>> UpdateDraftAsync(
        Guid draftId,
        string? token,
        RegisterHostDraftForm form,
        CancellationToken ct);

    Task<Result<HostDraftForEditResponse>> GetDraftForEditAsync(Guid draftId, string? token, CancellationToken ct);

    Task<Result<SimpleMessageResponse>> SetPasswordAsync(string? token, HostSetPasswordRequest request, CancellationToken ct);

    Task<Result<ModeratorHostDraftListResponse>> ListDraftsForModeratorAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken ct);

    Task<Result<ModeratorHostDraftDetailResponse>> GetDraftDetailForModeratorAsync(Guid draftId, CancellationToken ct);

    Task<Result<SimpleMessageResponse>> ModeratorReviewAsync(
        Guid draftId,
        Guid moderatorId,
        ModeratorReviewHostDraftRequest request,
        CancellationToken ct);
}
