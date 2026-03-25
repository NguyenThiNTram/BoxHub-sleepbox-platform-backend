using BoxHub.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BoxHub.Infrastructure.BackgroundJobs;

/// <summary>Hangfire job shell — tạo scope để chạy worker (DbContext scoped).</summary>
public sealed class CreateHostAccountJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CreateHostAccountJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Execute(Guid draftId, Guid moderatorId)
    {
        using var scope = _scopeFactory.CreateScope();
        var worker = scope.ServiceProvider.GetRequiredService<ICreateHostAccountWorker>();
        await worker.ExecuteAsync(draftId, moderatorId, CancellationToken.None);
    }
}
