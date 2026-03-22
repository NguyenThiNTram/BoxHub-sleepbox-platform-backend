namespace BoxHub.Application.Interfaces.Services;

/// <summary>Thực thi trong Hangfire sau khi moderator approve (transaction).</summary>
public interface ICreateHostAccountWorker
{
    Task ExecuteAsync(Guid draftId, Guid moderatorId, CancellationToken ct);
}
