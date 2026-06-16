namespace FireflyAPI.Application.Dtos;

public sealed class ProjectDetailsDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ActivityDto> Activities { get; set; } = [];
    public List<ResourceDto> Resources { get; set; } = [];
    
    public int ActivitiesCount { get; set; }
    public int ResourcesCount { get; set; }
}