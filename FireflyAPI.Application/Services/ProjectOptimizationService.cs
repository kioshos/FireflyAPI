using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Application.Optimization.Builders;
using FireflyAPI.Application.Optimization.Interfaces;

namespace FireflyAPI.Application.Services;

public class ProjectOptimizationService : IProjectOptimizationService
{
    private readonly IActivityRepository _activities;
    private readonly IResourceRepository _resources;
    private readonly IActivityDependencyRepository _dependencies;
    private readonly IResourceRequirementRepository _requirements;

    private readonly IProjectOptimizer _optimizer;
    
    public ProjectOptimizationService(IActivityRepository activities, IResourceRepository resources,
        IActivityDependencyRepository dependencies, IResourceRequirementRepository requirements,
        IProjectOptimizer optimizer)
    {
        _activities = activities;
        _resources = resources;
        _dependencies = dependencies;
        _requirements = requirements;
        _optimizer = optimizer;
    }

    public async Task<OptimizationResultDto> OptimizeProject(Guid projectId, CancellationToken ct)
    {
        var activities = await _activities.GetByProjectIdAsync(projectId, ct);

        var resources = await _resources.GetByProjectIdAsync(projectId, ct);
        
        var activityIds = activities.Select(a => a.Id).ToList();
        
        var dependencies = await _dependencies.GetByActivityIdsAsync(activityIds, ct);
        var requirements = await _requirements.GetByTaskIdsAsync(activityIds, ct);

        var context = new OptimizationContextBuilder()
                .Build(
                    activities,
                    resources,
                    dependencies,
                    requirements);

        var result = _optimizer.Optimize(context);

        return new OptimizationResultDto
        {
            Makespan = result.Makespan,

            Tasks = result.Schedule
                .Select(x => new OptimizedTaskDto
                {
                    ActivityId = x.Activity.OriginalId,
                    Name = x.Activity.Name,
                    Start = x.Start,
                    Finish = x.Finish
                })
                .ToList()
        };
    }
}