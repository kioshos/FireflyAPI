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
       
       var predecessor = await _activityRepository.GetByIdAsync(predecessorsRequestDto.PredecessorId, cancellationToken);
       if (predecessor == null)
           throw new Exception("Predecessor was not found!");
       
       if(predecessor.ProjectId != activity.ProjectId)
           throw new Exception("Activities from different project!");

       ActivityDependency activityDependency = new ActivityDependency(predecessorsRequestDto.ActivityId, predecessorsRequestDto.PredecessorId);

       await _activityDependencyRepository.AddAsync(activityDependency, cancellationToken);
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
}