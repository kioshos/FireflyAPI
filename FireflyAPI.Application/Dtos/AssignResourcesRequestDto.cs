namespace FireflyAPI.Application.Dtos;

public sealed class AssignResourcesRequestDto
{
    public Guid ActivityId { get; init; }
    public List<ResourceAssignmentDto> Resources { get; init; } = [];
}