using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using FireflyAPI.Application.Dtos;

namespace FireflyAPI.Application.Services;

public class ProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProjectService(IProjectRepository projectRepository, IActivityRepository activityRepository, 
        IResourceRepository resourceRepository, ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _activityRepository = activityRepository;
        _resourceRepository = resourceRepository;
        _currentUserService = currentUserService;
    }
    
    public async Task<IEnumerable<Project>> GetProjects(CancellationToken cancellationToken = default)
    {
        var targetProjects = await _projectRepository.GetAllAsync(cancellationToken);

        return targetProjects;
    }
    public async Task<IEnumerable<ProjectListItemDto>> GetUserProjects(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedAccessException();

        var projects = await _projectRepository.GetProjectsByUserId(userId, cancellationToken);

        var result = new List<ProjectListItemDto>();

        foreach (var project in projects)
        {
            var activities = await _activityRepository.GetByProjectIdAsync(project.Id, cancellationToken);
            var resources = await _resourceRepository.GetByProjectIdAsync(project.Id, cancellationToken);

            result.Add(new ProjectListItemDto
            {
                Id = project.Id,
                OwnerId = project.OwnerId,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,

                ActivitiesCount = activities?.Count() ?? 0,
                ResourcesCount = resources?.Count() ?? 0
            });
        }

        return result;
    }


    public async Task CreateProject(CreateProjectRequestDto request, CancellationToken cancellationToken = default)
    {
        var ownerId = _currentUserService.UserId
                          ?? throw new UnauthorizedAccessException();
        
        Project newProject = new Project(ownerId, request.Name, request.Description, request.StartTime);

        await _projectRepository.AddAsync(newProject, cancellationToken);
    }
    
    public async Task EditProject(Guid projectId, EditProjectRequestDto request, CancellationToken cancellationToken = default)
    {
        var targetProject = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (targetProject == null)
            throw new Exception("Project was not found!");

        targetProject.Name = request.Name;
        targetProject.Description = request.Description;

        await _projectRepository.UpdateAsync(targetProject, cancellationToken);
    }

    public async Task DeleteProject(Guid projectId, CancellationToken cancellationToken = default)
    {
        var targetProject = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (targetProject == null)
            throw new Exception("Project was not found!");

        await _projectRepository.DeleteAsync(targetProject, cancellationToken);
    }
    public async Task<ProjectDetailsDto> GetProject(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project == null)
            throw new Exception("Project was not found!");

        var activities = await _activityRepository.GetByProjectIdAsync(projectId, cancellationToken);

        var resources = await _resourceRepository.GetByProjectIdAsync(projectId, cancellationToken);

        int activitiesCount = activities?.Count() ?? 0;
        int resourceCount = resources?.Count() ?? 0;
        
        return new ProjectDetailsDto
        {
            Id = project.Id,
            OwnerId = project.OwnerId,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            StartTime = project.StartTime,

            Activities = activities
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Duration = a.Duration
                })  
                .ToList(),

            Resources = resources
                .Select(r => new ResourceDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Amount = r.Amount
                })
                .ToList(),
            
            ActivitiesCount = activitiesCount,
            ResourcesCount = resourceCount
        };
    }

}