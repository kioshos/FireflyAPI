namespace FireflyAPI.Application.Dtos;

public sealed class AssignPredecessorsRequestDto
{
    public Guid ActivityId { get; init; }
    public List<Guid> PredecessorIds { get; init; } = [];
}