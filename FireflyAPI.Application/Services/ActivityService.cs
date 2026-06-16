using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Services;

public class ActivityService
{
    private readonly IActivityDependencyRepository _activityDependencyRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IProjectRepository _projectRepository;

    public ActivityService(IActivityDependencyRepository activityDependencyRepository,
        IActivityRepository activityRepository, IProjectRepository projectRepository)
    {
        _activityDependencyRepository = activityDependencyRepository;
        _activityRepository = activityRepository;
        _projectRepository = projectRepository;
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

        var activity = new Activity(projectId, activityRequestDto.Duration, activityRequestDto.Name);

        await _activityRepository.AddAsync(activity, ct);

        return activity.Id;
    }
    
    public async Task AssignPredecessors(AssignPredecessorsRequestDto predecessorsRequestDto,
        CancellationToken cancellationToken = default)
    {
        var activity = await _activityRepository.GetByIdAsync(predecessorsRequestDto.ActivityId, cancellationToken);
        if (activity == null)
            throw new Exception("Activity was not found!");
       
        var predecessors = await _activityRepository.GetByIdsAsync(predecessorsRequestDto.PredecessorIds, 
            cancellationToken);
       
        if (predecessors.Count() != predecessorsRequestDto.PredecessorIds.Count)
            throw new Exception("One or more predecessors were not found.");
       
        foreach (var predecessor in predecessors)
        {
            if (predecessor.ProjectId != activity.ProjectId)
                throw new Exception("Activities from different project!");

            var dependency = new ActivityDependency(
                activity.Id,
                predecessor.Id);

            await _activityDependencyRepository.AddAsync(
                dependency,
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
    }
    public async Task Edit(Guid activityId, EditActivityRequestDto request, CancellationToken ct = default)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId, ct);

        if (activity == null)
            throw new Exception("Activity was not found!");

        activity.Name = request.Name;
        activity.Duration = request.Duration;

        await _activityRepository.UpdateAsync(activity, ct);
    }
    public async Task<IEnumerable<ActivityDetailDto>> GetActivities(Guid projectId, CancellationToken ct)
    {
        var activities = await _activityRepository.GetByProjectIdAsync(projectId, ct);

        var activityIds = activities.Select(a => a.Id).ToList();
        var dependencies = await _activityDependencyRepository.GetByActivityIdsAsync(activityIds, ct);

        var groupedDeps = dependencies
            .GroupBy(d => d.ActivityId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(d => d.PredecessorActivityId).ToList()
            );

        var predecessorIds = dependencies
            .Select(d => d.PredecessorActivityId)
            .Distinct()
            .ToList();
        
        var predecessorActivities = await _activityRepository.GetByIdsAsync(predecessorIds, ct);
        
        var predecessorMap = predecessorActivities.ToDictionary(p => p.Id);

        return activities.Select(a => new ActivityDetailDto
        {
            Id = a.Id,
            Name = a.Name,
            Duration = a.Duration,

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
                : new List<ActivityDto>()
        });
    }

}