using System.ComponentModel.Design;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using FireflyAPI .Application.Dtos;

namespace FireflyAPI.Application.Services;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IResourceRequirementRepository _resourceRequirementRepository;
    private readonly IRepository<Activity> _activityRepository;

    public ResourceService(IRepository<Resource> resourceRepository, 
        IProjectRepository projectRepository, IResourceRequirementRepository resourceRequirementRepository,
        IRepository<Activity> activityRepository)
    {
        _resourceRepository = resourceRepository;
        _projectRepository = projectRepository;
        _resourceRequirementRepository = resourceRequirementRepository;
        _activityRepository = activityRepository;
        
    }
    
    public async Task<IEnumerable<Resource>> GetProjectResources(Guid projectId, CancellationToken cancellationToken = default)
    {
        var resources = await _resourceRepository.GetAllAsync(cancellationToken);

        return resources.Where(r => r.ProjectId == projectId);
    }
    
    public async Task<Guid> CreateResource(Guid projectId, CreateResourceRequestDto request, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project == null)
            throw new Exception("Project was not found!");

        var resource = new Resource(projectId, request.Amount, request.Name);

        await _resourceRepository.AddAsync(resource, cancellationToken);

        return resource.Id;
    }
    
    public async Task EditResource(Guid resourceId, EditResourceRequestDto request, CancellationToken cancellationToken = default)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (resource == null)
            throw new Exception("Resource was not found!");

        resource.Name = request.Name;
        resource.Amount = request.Amount;

        await _resourceRepository.UpdateAsync(resource, cancellationToken);
    }
    
    public async Task DeleteResource(Guid resourceId, CancellationToken cancellationToken = default)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (resource == null)
            throw new Exception("Resource was not found!");

        await _resourceRepository.DeleteAsync(resource, cancellationToken);
    }

    public async Task AssignResourcesToActivity(AssignResourcesRequestDto assignResourcesRequestDto,
        CancellationToken cancellationToken = default)
    {
        var resource = await _resourceRepository.GetByIdAsync(assignResourcesRequestDto.ResourceId, cancellationToken);
        if (resource == null)
            throw new Exception("Resource was not found!");
        var activity = await _activityRepository.GetByIdAsync(assignResourcesRequestDto.ActivityId, cancellationToken);
        if (activity == null)
            throw new Exception("Activity was not found!");
        
        if(activity.ProjectId != resource.ProjectId)
            throw new Exception("Different project!");

        ResourceRequirement resourceRequirement = new ResourceRequirement(assignResourcesRequestDto.ActivityId,
            assignResourcesRequestDto.ResourceId, assignResourcesRequestDto.Amount);
        
        await _resourceRequirementRepository.AddAsync(resourceRequirement, cancellationToken);

    }
    
}