namespace FireflyAPI.Dtos;

public sealed class ProjectCreateDto
{
    public required string Name { get; init; }
    public string Description { get; init; }
}