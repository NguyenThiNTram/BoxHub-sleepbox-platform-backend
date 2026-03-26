namespace BoxHub.Application.Interfaces.Services;

/// <summary>Tầng Application không phụ thuộc Hangfire — abstraction enqueue job.</summary>
public interface IHostRegistrationJobClient
{
    void EnqueueCreateHostAccount(Guid draftId, Guid moderatorId);
}
