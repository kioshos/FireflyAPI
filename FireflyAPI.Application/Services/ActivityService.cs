using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Services;

public class ActivityService
{
    private readonly IActivityDependencyRepository _activityDependencyRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IResourceRequirementRepository _resourceRequirementRepository;
    private readonly IResourceRepository _resourceRepository;
    public ActivityService(IActivityDependencyRepository activityDependencyRepository,
        IActivityRepository activityRepository, IProjectRepository projectRepository,
        IResourceRequirementRepository resourceRequirementRepository, IResourceRepository resourceRepository)
    {
        _activityDependencyRepository = activityDependencyRepository;
        _activityRepository = activityRepository;
        _projectRepository = projectRepository;
        _resourceRequirementRepository = resourceRequirementRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<IEnumerable<Activity>> GetActivities(CancellationToken ct)
    {
        var result = await _activityRepository.GetAllAsync(ct);
        return result;
    }

    public async Task<IEnumerable<Activity>> GetActivitiesByProjectId(Guid projectId, CancellationToken ct)
    {
        var result = await _activityRepository.GetByProjectIdAsync(projectId, ct);
        return result;
    }
    public async Task<Guid> CreateActivity(Guid projectId,CreateActivityRequestDto activityRequestDto,
        CancellationToken ct =  default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct);

        if (project == null)
            throw new Exception("Project was not found!");

        var activity = new Activity(projectId, activityRequestDto.Duration, activityRequestDto.Name, activityRequestDto.RiskLevel);

        await _activityRepository.AddAsync(activity, ct);

        await RecalculateProjectRiskAsync(activity.ProjectId, ct);
        
        return activity.Id;
    }
    
    public async Task AssignPredecessors(AssignPredecessorsRequestDto predecessorsRequestDto,
        CancellationToken cancellationToken = default)
    {
        var activity = await _activityRepository.GetByIdAsync(predecessorsRequestDto.ActivityId, cancellationToken);
        if (activity == null)
            throw new Exception("Activity was not found!");

        await _activityDependencyRepository.DeleteByActivityIdAsync(predecessorsRequestDto.ActivityId, cancellationToken);

        var predecessors = await _activityRepository.GetByIdsAsync(predecessorsRequestDto.PredecessorIds, cancellationToken);

        if (predecessors.Count() != predecessorsRequestDto.PredecessorIds.Count)
            throw new Exception("One or more predecessors were not found.");

        foreach (var predecessor in predecessors)
        {
            if (predecessor.ProjectId != activity.ProjectId)
                throw new Exception("Activities from different project!");

            await _activityDependencyRepository.AddAsync(
                new ActivityDependency(activity.Id, predecessor.Id),
                cancellationToken);
        }
    }

    public async Task<ActivityDetailDto> GetActivityDetails(Guid activityId,
        CancellationToken cancellationToken = default)
    {
        
        var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        if (activity == null)
            throw new Exception("Activity was not found!");

        var dependencies = await _activityDependencyRepository.GetByActivityIdAsync(activityId, cancellationToken);

        var predecessorIds = dependencies.Select(d => d.PredecessorActivityId).ToList();
        
        var predecessorActivities = await _activityRepository.GetByIdsAsync(predecessorIds, cancellationToken);
        
        var predecessors = predecessorActivities
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name,
                Duration = a.Duration
            })
            .ToList();
        
        return new ActivityDetailDto
        {
            Id = activity.Id,
            Name = activity.Name,
            Duration = activity.Duration,
            Predecessors = predecessors
        };
    }
    public async Task DeleteActivity(Guid activityId, CancellationToken cancellationToken = default)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        if (activity == null)
            throw new Exception("Activity was not found!");

        await _activityRepository.DeleteAsync(activity, cancellationToken);
        
        await RecalculateProjectRiskAsync(activity.ProjectId, cancellationToken);
    }
    public async Task Edit(Guid activityId, EditActivityRequestDto request, CancellationToken ct = default)
    {
       var activity = await _activityRepository.GetByIdAsync(activityId, ct);

        if (activity == null)
            throw new Exception("Activity was not found!");

        activity.Name = request.Name;
        activity.Duration = request.Duration;
        activity.RiskLevel = request.RiskLevel; 
        
        await _activityRepository.UpdateAsync(activity, ct);
        
        await RecalculateProjectRiskAsync(activity.ProjectId, ct);
    }
      public async Task<IEnumerable<ActivityDetailDto>> GetActivities(Guid projectId, CancellationToken ct)
    {
        var activities = await _activityRepository.GetByProjectIdAsync(projectId, ct);

        var activityIds = activities.Select(a => a.Id).ToList();
        var dependencies = await _activityDependencyRepository.GetByActivityIdsAsync(activityIds, ct);
        var resourceRequirements = await _resourceRequirementRepository.GetByTaskIdsAsync(activityIds, ct);

        var groupedDeps = dependencies
            .GroupBy(d => d.ActivityId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(d => d.PredecessorActivityId).ToList()
            );
        
        var groupedResources = resourceRequirements
            .GroupBy(r => r.ActivityId)
            .ToDictionary(
                x => x.Key,
                x => x.ToList()
            );

        var predecessorIds = dependencies
            .Select(d => d.PredecessorActivityId)
            .Distinct()
            .ToList();
        
        var resourceIds = resourceRequirements
            .Select(r => r.ResourceId)
            .Distinct()
            .ToList();

        var resources = await _resourceRepository.GetByIdsAsync(resourceIds, ct);

        var resourceMap = resources.ToDictionary(r => r.Id);
        
        var predecessorActivities = await _activityRepository.GetByIdsAsync(predecessorIds, ct);
        
        var predecessorMap = predecessorActivities.ToDictionary(p => p.Id);

        return activities.Select(a => new ActivityDetailDto
        {
            Id = a.Id,
            Name = a.Name,
            Duration = a.Duration,
            RiskLevel = a.RiskLevel,
            Predecessors = groupedDeps.ContainsKey(a.Id)
                ? groupedDeps[a.Id]
                    .Where(id => predecessorMap.ContainsKey(id))
                    .Select(id => predecessorMap[id])
                    .Select(p => new ActivityDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Duration = p.Duration
                    })
                    .ToList()
                : new List<ActivityDto>(),
            
            Resources = groupedResources.ContainsKey(a.Id)
                ? groupedResources[a.Id]
                    .Where(r => resourceMap.ContainsKey(r.ResourceId))
                    .Select(r => new ResourceDto
                    {
                        Id = r.ResourceId,
                        Name = resourceMap[r.ResourceId].Name,
                        Amount = r.Amount
                    })
                    .ToList()
                : new List<ResourceDto>()
        });
    }
    private async Task RecalculateProjectRiskAsync(Guid projectId, CancellationToken ct)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct);
        if (project == null) return;

        var allActivities = await _activityRepository.GetByProjectIdAsync(projectId, ct);

        double calculatedRisk = 0;
        if (allActivities.Any())
        {
            double totalDuration = allActivities.Sum(a => a.Duration);
        
            if (totalDuration > 0)
            {
                calculatedRisk = allActivities.Sum(a => a.RiskLevel * a.Duration) / totalDuration;
            }
            else
            {
                calculatedRisk = allActivities.Average(a => a.RiskLevel);
            }
        }

        project.TotalRiskLevel = Math.Round(calculatedRisk, 2);
        await _projectRepository.UpdateAsync(project, ct);
    }


}