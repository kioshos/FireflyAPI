using FireflyAPI.Application.Dtos;

namespace FireflyAPI.Application.Interfaces;

public interface IProjectOptimizationService
{
    Task<OptimizationResultDto> OptimizeProject(Guid projectId, CancellationToken ct);
}