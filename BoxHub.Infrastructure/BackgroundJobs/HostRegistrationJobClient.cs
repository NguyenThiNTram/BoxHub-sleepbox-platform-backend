using BoxHub.Application.Interfaces.Services;
using Hangfire;

namespace BoxHub.Infrastructure.BackgroundJobs;

public sealed class HostRegistrationJobClient : IHostRegistrationJobClient
{
    public void EnqueueCreateHostAccount(Guid draftId, Guid moderatorId)
    {
        BackgroundJob.Enqueue<CreateHostAccountJob>(j => j.Execute(draftId, moderatorId));
    }
}
