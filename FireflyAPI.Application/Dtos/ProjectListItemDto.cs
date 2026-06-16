namespace FireflyAPI.Application.Dtos;

public class ProjectListItemDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public int ActivitiesCount { get; set; } 
    public int ResourcesCount { get; set; }
}