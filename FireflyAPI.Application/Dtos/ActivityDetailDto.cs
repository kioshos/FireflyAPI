namespace FireflyAPI.Application.Dtos;

public sealed class ActivityDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }

    public List<ActivityDto> Predecessors { get; set; } = [];
    public List<ResourceDto> Resources { get; set; } = [];
}