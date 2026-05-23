namespace FireflyAPI.Application.Dtos;

public sealed class AssignResourcesRequestDto
{
    public Guid ActivityId { get; init; }
    public Guid ResourceId { get; init; }
    public double Amount { get; init; }
}