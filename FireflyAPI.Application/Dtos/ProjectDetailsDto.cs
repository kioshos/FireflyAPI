namespace FireflyAPI.Application.Dtos;

public sealed class ProjectDetailsDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public double TotalRiskLevel { get; set; }
    public string TotalRiskLevelLabel => TotalRiskLevel switch
    {
        < 0.25 => "Low",
        < 0.5 => "Medium",
        < 0.75 => "High",
        _ => "Critical"
    };
    public List<ActivityDto> Activities { get; set; } = [];
    public List<ResourceDto> Resources { get; set; } = [];
    
    public int ActivitiesCount { get; set; }
    public int ResourcesCount { get; set; }
}